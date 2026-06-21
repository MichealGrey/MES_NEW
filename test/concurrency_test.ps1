# MES System Concurrency Stress Test
# Test: 50 concurrent users

$baseUrl = "http://localhost:8940"

# Login to get token
Write-Host "=== Preparing: Get Auth Token ===" -ForegroundColor Cyan
$loginResp = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"userId":"USR-ADMIN","password":"admin123"}'
$token = $loginResp.data.token
$headers = @{ "Authorization" = "Bearer $token"; "Content-Type" = "application/json" }
Write-Host "Token acquired" -ForegroundColor Green

# Test function
function Test-ApiEndpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [string]$Body = $null,
        [int]$Concurrency = 50
    )

    Write-Host "`n=== Testing: $Name (Concurrency: $Concurrency) ===" -ForegroundColor Yellow

    $scriptBlock = {
        param($method, $url, $headers, $body)
        try {
            $sw = [System.Diagnostics.Stopwatch]::StartNew()
            if ($body) {
                $resp = Invoke-WebRequest -Uri $url -Method $method -Headers $headers -Body $body -UseBasicParsing -TimeoutSec 30
            } else {
                $resp = Invoke-WebRequest -Uri $url -Method $method -Headers $headers -UseBasicParsing -TimeoutSec 30
            }
            $sw.Stop()
            return @{ Success = $true; StatusCode = $resp.StatusCode; ResponseTime = $sw.ElapsedMilliseconds; Error = $null }
        } catch {
            return @{ Success = $false; StatusCode = 0; ResponseTime = 0; Error = $_.Exception.Message }
        }
    }

    $jobs = @()
    for ($i = 0; $i -lt $Concurrency; $i++) {
        $jobs += Start-Job -ScriptBlock $scriptBlock -ArgumentList $Method, $Url, $Headers, $Body
    }

    Write-Host "Waiting for tasks..." -ForegroundColor Gray
    $jobs | Wait-Job | Out-Null

    $results = @()
    foreach ($job in $jobs) {
        $results += Receive-Job -Job $job
        Remove-Job -Job $job
    }

    $succ = ($results | Where-Object { $_.Success }).Count
    $fail = ($results | Where-Object { -not $_.Success }).Count
    $times = $results | Where-Object { $_.Success } | Select-Object -ExpandProperty ResponseTime
    $avg = if ($times.Count -gt 0) { [math]::Round(($times | Measure-Object -Average).Average, 0) } else { 0 }
    $max = if ($times.Count -gt 0) { ($times | Measure-Object -Maximum).Maximum } else { 0 }
    $min = if ($times.Count -gt 0) { ($times | Measure-Object -Minimum).Minimum } else { 0 }
    $p95 = if ($times.Count -gt 0) { $s = $times | Sort-Object; $s[[math]::Floor($s.Count * 0.95)] } else { 0 }

    $rate = [math]::Round($succ/$Concurrency*100, 1)
    Write-Host "  Success: $succ/$Concurrency ($rate pct)" -ForegroundColor $(if ($succ -eq $Concurrency) { "Green" } else { "Red" })
    Write-Host "  Failed: $fail" -ForegroundColor $(if ($fail -eq 0) { "Green" } else { "Red" })
    Write-Host "  Avg: ${avg}ms | P95: ${p95}ms | Max: ${max}ms | Min: ${min}ms" -ForegroundColor White

    if ($fail -gt 0) {
        $errs = $results | Where-Object { -not $_.Success } | Select-Object -ExpandProperty Error -Unique
        Write-Host "  Errors:" -ForegroundColor Red
        foreach ($e in $errs | Select-Object -First 3) {
            Write-Host "    - $e" -ForegroundColor Red
        }
    }

    return @{
        TestName = $Name
        Concurrency = $Concurrency
        SuccessCount = $succ
        FailCount = $fail
        AvgResponseTime = $avg
        P95ResponseTime = $p95
        MaxResponseTime = $max
        MinResponseTime = $min
        SuccessRate = $rate
    }
}

# ========================================
# Start Tests
# ========================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   MES Concurrency Stress Test" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$allResults = @()

# Test 1: WorkOrder list (READ)
$allResults += Test-ApiEndpoint `
    -Name "WorkOrder List (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/WorkOrders?pageIndex=1&pageSize=20" `
    -Headers $headers `
    -Concurrency 50

# Test 2: Lots list (READ)
$allResults += Test-ApiEndpoint `
    -Name "Lots List (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/Lots?pageIndex=1&pageSize=20" `
    -Headers $headers `
    -Concurrency 50

# Test 3: Products (READ)
$allResults += Test-ApiEndpoint `
    -Name "Products (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/MasterData/products" `
    -Headers $headers `
    -Concurrency 50

# Test 4: Equipment (READ)
$allResults += Test-ApiEndpoint `
    -Name "Equipment List (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/Equipment?pageIndex=1&pageSize=50" `
    -Headers $headers `
    -Concurrency 50

# Test 5: Routes (READ)
$allResults += Test-ApiEndpoint `
    -Name "Routes (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/Routes" `
    -Headers $headers `
    -Concurrency 50

# Test 6: Report (COMPLEX QUERY)
$allResults += Test-ApiEndpoint `
    -Name "Production Report (GET)" `
    -Method "GET" `
    -Url "$baseUrl/api/Report/production?dateFrom=2026-06-01&dateTo=2026-06-18" `
    -Headers $headers `
    -Concurrency 30

# Test 7: Trace query
$lotsResp = Invoke-RestMethod -Uri "$baseUrl/api/Lots?pageIndex=1&pageSize=1" -Headers $headers
if ($lotsResp.data.items.Count -gt 0) {
    $testLotId = $lotsResp.data.items[0].lotId

    $allResults += Test-ApiEndpoint `
        -Name "Forward Trace (GET)" `
        -Method "GET" `
        -Url "$baseUrl/api/Trace/forward/$testLotId" `
        -Headers $headers `
        -Concurrency 30

    $allResults += Test-ApiEndpoint `
        -Name "Backward Trace (GET)" `
        -Method "GET" `
        -Url "$baseUrl/api/Trace/backward/$testLotId" `
        -Headers $headers `
        -Concurrency 30
}

# Test 8: Create WorkOrder (WRITE - concurrent)
Write-Host "`n=== Testing: Create WorkOrder (POST) - Concurrent Write ===" -ForegroundColor Yellow
$createJobs = @()
for ($i = 0; $i -lt 20; $i++) {
    $idx = $i
    $body = @{
        woType = "Parent"
        productId = "PROD-QFN88"
        routeId = "QFN-STD:1.0"
        plannedQty = 10000
        waferQty = 1
        unitQty = 1
        customerId = "CUST-AUTO"
        priority = "Normal"
        plannedStartDate = "2026-07-01T08:00:00"
        plannedEndDate = "2026-07-15T18:00:00"
        remark = "Concurrency test WO-$idx"
    } | ConvertTo-Json -Compress

    $createJobs += Start-Job -ScriptBlock {
        param($url, $headers, $body)
        try {
            $sw = [System.Diagnostics.Stopwatch]::StartNew()
            $resp = Invoke-WebRequest -Uri $url -Method POST -Headers $headers -Body $body -ContentType "application/json" -UseBasicParsing -TimeoutSec 30
            $sw.Stop()
            return @{ Success = $true; StatusCode = $resp.StatusCode; ResponseTime = $sw.ElapsedMilliseconds; Error = $null }
        } catch {
            return @{ Success = $false; StatusCode = 0; ResponseTime = 0; Error = $_.Exception.Message }
        }
    } -ArgumentList "$baseUrl/api/WorkOrders", $headers, $body
}

$createJobs | Wait-Job | Out-Null
$createResults = @()
foreach ($job in $createJobs) {
    $createResults += Receive-Job -Job $job
    Remove-Job -Job $job
}

$cs = ($createResults | Where-Object { $_.Success }).Count
$cf = ($createResults | Where-Object { -not $_.Success }).Count
$ct = $createResults | Where-Object { $_.Success } | Select-Object -ExpandProperty ResponseTime
$cavg = if ($ct.Count -gt 0) { [math]::Round(($ct | Measure-Object -Average).Average, 0) } else { 0 }
$cp95 = if ($ct.Count -gt 0) { $s = $ct | Sort-Object; $s[[math]::Floor($s.Count * 0.95)] } else { 0 }
$cmax = if ($ct.Count -gt 0) { ($ct | Measure-Object -Maximum).Maximum } else { 0 }
$cmin = if ($ct.Count -gt 0) { ($ct | Measure-Object -Minimum).Minimum } else { 0 }
$crate = [math]::Round($cs/20*100, 1)

Write-Host "  Success: $cs/20 ($crate pct)" -ForegroundColor $(if ($cs -eq 20) { "Green" } else { "Red" })
Write-Host "  Failed: $cf" -ForegroundColor $(if ($cf -eq 0) { "Green" } else { "Red" })
Write-Host "  Avg: ${cavg}ms | P95: ${cp95}ms | Max: ${cmax}ms | Min: ${cmin}ms" -ForegroundColor White

$allResults += @{
    TestName = "Create WorkOrder (Write)"
    Concurrency = 20
    SuccessCount = $cs
    FailCount = $cf
    AvgResponseTime = $cavg
    P95ResponseTime = $cp95
    MaxResponseTime = $cmax
    MinResponseTime = $cmin
    SuccessRate = $crate
}

# ========================================
# Generate Report
# ========================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "   Concurrency Test Report" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$allResults | Format-Table -AutoSize -Property @(
    @{Label="Test"; Expression={$_.TestName}},
    @{Label="Conc"; Expression={$_.Concurrency}},
    @{Label="OK"; Expression={$_.SuccessCount}},
    @{Label="Fail"; Expression={$_.FailCount}},
    @{Label="Rate"; Expression={"$($_.SuccessRate)%"}},
    @{Label="Avg(ms)"; Expression={$_.AvgResponseTime}},
    @{Label="P95(ms)"; Expression={$_.P95ResponseTime}},
    @{Label="Max(ms)"; Expression={$_.MaxResponseTime}}
)

# Summary
$totalTests = $allResults.Count
$passed = ($allResults | Where-Object { $_.SuccessRate -eq 100 }).Count
$failed = $totalTests - $passed
$overallAvg = [math]::Round(($allResults | Measure-Object -Property AvgResponseTime -Average).Average, 0)
$overallP95 = ($allResults | Sort-Object P95ResponseTime -Descending | Select-Object -First 1).P95ResponseTime

Write-Host "`n=== Summary ===" -ForegroundColor Cyan
Write-Host "Total: $totalTests | Passed: $passed | Failed: $failed" -ForegroundColor White
Write-Host "Overall Avg: ${overallAvg}ms | Worst P95: ${overallP95}ms" -ForegroundColor White

if ($overallP95 -le 500 -and $failed -eq 0) {
    Write-Host "`nRating: EXCELLENT - Production ready" -ForegroundColor Green
} elseif ($overallP95 -le 1000 -and $failed -le 2) {
    Write-Host "`nRating: GOOD - Basic requirements met, optimization recommended" -ForegroundColor Yellow
} else {
    Write-Host "`nRating: NEEDS IMPROVEMENT - Performance issues detected" -ForegroundColor Red
}

# Save report
$reportPath = "e:\AiProj\MES_NEW\docs\MES_Concurrency_Test_Report.md"
$lines = @()
$lines += "# MES Concurrency Stress Test Report"
$lines += ""
$lines += "**Date:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$lines += "**Env:** Windows + MySQL + .NET 8.0"
$lines += "**API:** $baseUrl"
$lines += ""
$lines += "## Summary"
$lines += ""
$lines += "| Metric | Value |"
$lines += "|--------|-------|"
$lines += "| Total Tests | $totalTests |"
$lines += "| Passed | $passed |"
$lines += "| Failed | $failed |"
$lines += "| Avg Response | ${overallAvg}ms |"
$lines += "| Worst P95 | ${overallP95}ms |"
$lines += ""
$lines += "## Results"
$lines += ""
$lines += "| Test | Conc | OK | Fail | Rate | Avg(ms) | P95(ms) | Max(ms) |"
$lines += "|------|------|----|------|------|---------|---------|---------|"
foreach ($r in $allResults) {
    $lines += "| $($r.TestName) | $($r.Concurrency) | $($r.SuccessCount) | $($r.FailCount) | $($r.SuccessRate)% | $($r.AvgResponseTime) | $($r.P95ResponseTime) | $($r.MaxResponseTime) |"
}
$lines += ""
$rating = if ($overallP95 -le 500 -and $failed -eq 0) { "EXCELLENT" } elseif ($overallP95 -le 1000 -and $failed -le 2) { "GOOD" } else { "NEEDS IMPROVEMENT" }
$lines += "## Rating: $rating"
$lines += ""
$lines += "## Recommendations"
$lines += "1. Optimize DB connection pool"
$lines += "2. Add query caching (Redis)"
$lines += "3. Add indexes for complex queries"
$lines += "4. Add optimistic locking for write operations"
$lines += "5. Implement API rate limiting"

$lines | Out-File -FilePath $reportPath -Encoding UTF8
Write-Host "`nReport saved: $reportPath" -ForegroundColor Green
