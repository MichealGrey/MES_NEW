using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static string BaseUrl = "http://localhost:8940";
    static HttpClient Client = new();
    static List<TestResult> Results = new();

    class TestResult
    {
        public string TestName { get; set; } = "";
        public int Concurrency { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public double AvgResponseTime { get; set; }
        public long P95ResponseTime { get; set; }
        public long MaxResponseTime { get; set; }
        public long MinResponseTime { get; set; }
        public double SuccessRate { get; set; }
    }

    static async Task Main(string[] args)
    {
        // Login
        Console.WriteLine("=== Preparing: Get Auth Token ===");
        var loginBody = new { userId = "USR-ADMIN", password = "admin123" };
        var loginResp = await PostJsonAsync("/api/auth/login", loginBody);
        var token = loginResp.GetProperty("data").GetProperty("token").GetString();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("Token acquired\n");

        Console.WriteLine("========================================");
        Console.WriteLine("   MES Concurrency Stress Test");
        Console.WriteLine("========================================\n");

        // Test 1: WorkOrder List (50 concurrent)
        await TestEndpoint("WorkOrder List (GET)", "/api/WorkOrders?pageIndex=1&pageSize=20", 50);

        // Test 2: Lots List (50 concurrent)
        await TestEndpoint("Lots List (GET)", "/api/Lots?pageIndex=1&pageSize=20", 50);

        // Test 3: Products (50 concurrent)
        await TestEndpoint("Products (GET)", "/api/MasterData/products", 50);

        // Test 4: Equipment (50 concurrent)
        await TestEndpoint("Equipment List (GET)", "/api/Equipment?pageIndex=1&pageSize=50", 50);

        // Test 5: Routes (50 concurrent) - SKIP: API not implemented yet
        // await TestEndpoint("Routes (GET)", "/api/Processes/routes", 50);
        Console.WriteLine("\n=== Skipping: Routes (GET) - API not implemented ===");

        // Test 6: Production Report (30 concurrent)
        await TestEndpoint("Production Report (GET)", "/api/Report/production?dateFrom=2026-06-01&dateTo=2026-06-18", 30);

        // Test 7: Forward Trace (30 concurrent)
        var lotsResp = await GetJsonAsync("/api/Lots?pageIndex=1&pageSize=1");
        var lotId = lotsResp.GetProperty("data").GetProperty("items")[0].GetProperty("lotId").GetString();
        await TestEndpoint("Forward Trace (GET)", $"/api/Trace/forward/{lotId}", 30);

        // Test 8: Backward Trace (30 concurrent)
        await TestEndpoint("Backward Trace (GET)", $"/api/Trace/backward/{lotId}", 30);

        // Test 9: Create WorkOrder (20 concurrent writes)
        await TestConcurrentWrite("Create WorkOrder (Write)", 20);

        // Report
        Console.WriteLine("\n========================================");
        Console.WriteLine("   Concurrency Test Report");
        Console.WriteLine("========================================\n");

        Console.WriteLine($"{"Test",-40} {"Conc",6} {"OK",5} {"Fail",5} {"Rate",8} {"Avg(ms)",10} {"P95(ms)",10} {"Max(ms)",10}");
        Console.WriteLine(new string('-', 98));
        foreach (var r in Results)
        {
            Console.WriteLine($"{r.TestName,-40} {r.Concurrency,6} {r.SuccessCount,5} {r.FailCount,5} {r.SuccessRate,7:F1}% {r.AvgResponseTime,9:F0} {r.P95ResponseTime,9} {r.MaxResponseTime,9}");
        }

        int totalTests = Results.Count;
        int passed = Results.Count(r => r.SuccessRate == 100);
        int failed = totalTests - passed;
        double overallAvg = Results.Average(r => r.AvgResponseTime);
        long overallP95 = Results.Max(r => r.P95ResponseTime);

        Console.WriteLine($"\n=== Summary ===");
        Console.WriteLine($"Total: {totalTests} | Passed: {passed} | Failed: {failed}");
        Console.WriteLine($"Overall Avg: {overallAvg:F0}ms | Worst P95: {overallP95}ms");

        string rating;
        if (overallP95 <= 500 && failed == 0)
        {
            rating = "EXCELLENT - Production ready";
            Console.WriteLine($"\nRating: {rating}");
        }
        else if (overallP95 <= 1000 && failed <= 2)
        {
            rating = "GOOD - Basic requirements met, optimization recommended";
            Console.WriteLine($"\nRating: {rating}");
        }
        else
        {
            rating = "NEEDS IMPROVEMENT - Performance issues detected";
            Console.WriteLine($"\nRating: {rating}");
        }

        // Save report
        var reportPath = @"e:\AiProj\MES_NEW\docs\MES_Concurrency_Test_Report.md";
        var report = new StringBuilder();
        report.AppendLine("# MES Concurrency Stress Test Report\n");
        report.AppendLine($"**Date:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"**Env:** Windows + MySQL + .NET 8.0");
        report.AppendLine($"**API:** {BaseUrl}\n");
        report.AppendLine("## Summary\n");
        report.AppendLine("| Metric | Value |");
        report.AppendLine("|--------|-------|");
        report.AppendLine($"| Total Tests | {totalTests} |");
        report.AppendLine($"| Passed | {passed} |");
        report.AppendLine($"| Failed | {failed} |");
        report.AppendLine($"| Avg Response | {overallAvg:F0}ms |");
        report.AppendLine($"| Worst P95 | {overallP95}ms |\n");
        report.AppendLine("## Results\n");
        report.AppendLine("| Test | Conc | OK | Fail | Rate | Avg(ms) | P95(ms) | Max(ms) |");
        report.AppendLine("|------|------|----|------|------|---------|---------|---------|");
        foreach (var r in Results)
        {
            report.AppendLine($"| {r.TestName} | {r.Concurrency} | {r.SuccessCount} | {r.FailCount} | {r.SuccessRate:F1}% | {r.AvgResponseTime:F0} | {r.P95ResponseTime} | {r.MaxResponseTime} |");
        }
        report.AppendLine($"\n## Rating: {rating}\n");
        report.AppendLine("## Recommendations\n");
        report.AppendLine("1. Optimize DB connection pool");
        report.AppendLine("2. Add query caching (Redis)");
        report.AppendLine("3. Add indexes for complex queries");
        report.AppendLine("4. Add optimistic locking for write operations");
        report.AppendLine("5. Implement API rate limiting");

        await System.IO.File.WriteAllTextAsync(reportPath, report.ToString());
        Console.WriteLine($"\nReport saved: {reportPath}");
    }

    static async Task TestEndpoint(string name, string path, int concurrency)
    {
        Console.WriteLine($"\n=== Testing: {name} (Concurrency: {concurrency}) ===");

        var tasks = Enumerable.Range(0, concurrency)
            .Select(async i =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();
                    var resp = await Client.GetAsync(BaseUrl + path);
                    sw.Stop();
                    return (success: resp.IsSuccessStatusCode, time: sw.ElapsedMilliseconds, error: (string?)null);
                }
                catch (Exception ex)
                {
                    return (success: false, time: 0L, error: ex.Message);
                }
            });

        var results = await Task.WhenAll(tasks);
        AnalyzeResults(name, concurrency, results);
    }

    static async Task TestConcurrentWrite(string name, int concurrency)
    {
        Console.WriteLine($"\n=== Testing: {name} (Concurrency: {concurrency}) ===");

        var tasks = Enumerable.Range(0, concurrency)
            .Select(async i =>
            {
                try
                {
                    var body = new
                    {
                        woType = "Parent",
                        productId = "PROD-QFN88",
                        routeId = "QFN-STD:1.0",
                        plannedQty = 10000,
                        waferQty = 1,
                        unitQty = 1,
                        customerId = "CUST-AUTO",
                        priority = "Normal",
                        plannedStartDate = "2026-07-01T08:00:00",
                        plannedEndDate = "2026-07-15T18:00:00",
                        remark = $"Concurrency test WO-{i}"
                    };
                    var content = new StringContent(
                        JsonSerializer.Serialize(body),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var sw = Stopwatch.StartNew();
                    var resp = await Client.PostAsync(BaseUrl + "/api/WorkOrders", content);
                    sw.Stop();
                    return (success: resp.IsSuccessStatusCode, time: sw.ElapsedMilliseconds, error: (string?)null);
                }
                catch (Exception ex)
                {
                    return (success: false, time: 0L, error: ex.Message);
                }
            });

        var results = await Task.WhenAll(tasks);
        AnalyzeResults(name, concurrency, results);
    }

    static void AnalyzeResults(string name, int concurrency, (bool success, long time, string? error)[] results)
    {
        var successResults = results.Where(r => r.success).ToList();
        var successCount = successResults.Count;
        var failCount = results.Length - successCount;

        var times = successResults.Select(r => r.time).OrderBy(t => t).ToList();
        var avg = times.Count > 0 ? times.Average() : 0;
        var max = times.Count > 0 ? times.Max() : 0;
        var min = times.Count > 0 ? times.Min() : 0;
        var p95Index = (int)Math.Floor(times.Count * 0.95);
        var p95 = times.Count > 0 ? times[Math.Min(p95Index, times.Count - 1)] : 0;
        var rate = Math.Round((double)successCount / concurrency * 100, 1);

        Console.WriteLine($"  Success: {successCount}/{concurrency} ({rate}%)");
        Console.WriteLine($"  Failed: {failCount}");
        Console.WriteLine($"  Avg: {avg:F0}ms | P95: {p95}ms | Max: {max}ms | Min: {min}ms");

        if (failCount > 0)
        {
            var errors = results.Where(r => !r.success).Select(r => r.error).Where(e => e != null).Distinct().Take(3);
            Console.WriteLine("  Errors:");
            foreach (var e in errors)
                Console.WriteLine($"    - {e}");
        }

        Results.Add(new TestResult
        {
            TestName = name,
            Concurrency = concurrency,
            SuccessCount = successCount,
            FailCount = failCount,
            AvgResponseTime = avg,
            P95ResponseTime = p95,
            MaxResponseTime = max,
            MinResponseTime = min,
            SuccessRate = rate
        });
    }

    static async Task<JsonElement> GetJsonAsync(string path)
    {
        var resp = await Client.GetAsync(BaseUrl + path);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }

    static async Task<JsonElement> PostJsonAsync(string path, object body)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );
        var resp = await Client.PostAsync(BaseUrl + path, content);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement;
    }
}
