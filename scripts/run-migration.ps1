param(
    [string]$Server = "localhost",
    [string]$Database = "mes_prod",
    [string]$User = "root",
    [string]$Password = "MyNewPass123!",
    [string]$SqlFile = "docs\sql\migrations\V2.0.0__master_data_expansion.sql"
)

# Find MySQL command line client
$mysqlPaths = @(
    "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
    "C:\Program Files\MySQL\MySQL Server 8.4\bin\mysql.exe",
    "C:\Program Files (x86)\MySQL\MySQL Server 8.0\bin\mysql.exe",
    "C:\mysql\bin\mysql.exe"
)

$mysqlExe = $null
foreach ($path in $mysqlPaths) {
    if (Test-Path $path) {
        $mysqlExe = $path
        break
    }
}

if (-not $mysqlExe) {
    # Try to find mysql in PATH or common locations
    $mysqlExe = Get-Command mysql -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
}

if (-not $mysqlExe) {
    Write-Error "MySQL client not found. Please install MySQL CLI or add to PATH."
    exit 1
}

Write-Host "Using MySQL client: $mysqlExe"
Write-Host "Executing SQL file: $SqlFile"

if (-not (Test-Path $SqlFile)) {
    Write-Error "SQL file not found: $SqlFile"
    exit 1
}

$sqlContent = Get-Content $SqlFile -Raw

# Split by semicolons to execute each statement separately
$statements = $sqlContent -split ';' | Where-Object { $_.Trim() -ne '' -and $_.Trim() -notmatch '^\s*--' }

$successCount = 0
$errorCount = 0

foreach ($stmt in $statements) {
    $cleanStmt = $stmt.Trim()
    if ([string]::IsNullOrWhiteSpace($cleanStmt)) { continue }
    if ($cleanStmt.StartsWith('--')) { continue }
    
    # Skip empty statements
    if ($cleanStmt -match '^\s*$') { continue }
    
    $tempFile = [System.IO.Path]::GetTempFileName()
    Set-Content -Path $tempFile -Value "$cleanStmt;" -Encoding UTF8
    
    try {
        $result = & $mysqlExe -h $Server -u $User -p$Password $Database -e "source $tempFile" 2>&1
        if ($LASTEXITCODE -eq 0) {
            $successCount++
        } else {
            # Check if error is just "table exists" or "column exists" which we can ignore
            $errorOutput = ($result | Out-String)
            if ($errorOutput -match 'already exists|Duplicate column|Duplicate key') {
                Write-Host "[SKIP] $successCount+1: $errorOutput" -ForegroundColor Yellow
                $successCount++
            } else {
                Write-Host "[ERROR] Statement failed: $($cleanStmt.Substring(0, [Math]::Min(80, $cleanStmt.Length)))..." -ForegroundColor Red
                Write-Host "  Error: $errorOutput" -ForegroundColor Red
                $errorCount++
            }
        }
    } catch {
        Write-Host "[EXCEPTION] $_" -ForegroundColor Red
        $errorCount++
    } finally {
        Remove-Item $tempFile -ErrorAction SilentlyContinue
    }
}

Write-Host ""
Write-Host "Migration completed!" -ForegroundColor Green
Write-Host "  Success: $successCount" -ForegroundColor Green
Write-Host "  Errors:  $errorCount" -ForegroundColor $(if ($errorCount -gt 0) { "Red" } else { "Green" })
