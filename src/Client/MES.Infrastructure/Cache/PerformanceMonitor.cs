using System.Collections.Concurrent;
using System.Diagnostics;

namespace MES.Infrastructure.Cache;

/// <summary>
/// Lightweight performance monitor for storage operations.
/// Tracks operation duration, cache hit/miss rates, and slow queries.
/// </summary>
public static class PerformanceMonitor
{
    private static readonly ConcurrentDictionary<string, OperationStats> _stats = new();
    private static readonly ConcurrentQueue<SlowQueryLog> _slowQueries = new();

    /// <summary>
    /// Threshold in milliseconds for slow query detection.
    /// </summary>
    public static int SlowQueryThresholdMs { get; set; } = 100;

    /// <summary>
    /// Record a storage operation with timing.
    /// </summary>
    public static void RecordOperation(string operation, long elapsedMs, bool cacheHit = false)
    {
        var stats = _stats.GetOrAdd(operation, _ => new OperationStats());
        stats.TotalCalls++;
        stats.TotalElapsedMs += elapsedMs;
        if (cacheHit) stats.CacheHits++;

        if (elapsedMs > SlowQueryThresholdMs)
        {
            _slowQueries.Enqueue(new SlowQueryLog
            {
                Operation = operation,
                ElapsedMs = elapsedMs,
                Timestamp = DateTime.UtcNow
            });

            // Keep only last 100 slow queries
            while (_slowQueries.Count > 100)
            {
                _slowQueries.TryDequeue(out _);
            }
        }
    }

    /// <summary>
    /// Get statistics for all operations.
    /// </summary>
    public static Dictionary<string, OperationStats> GetStats()
    {
        return _stats.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    /// <summary>
    /// Get recent slow queries.
    /// </summary>
    public static List<SlowQueryLog> GetSlowQueries()
    {
        return _slowQueries.ToList();
    }

    /// <summary>
    /// Reset all statistics.
    /// </summary>
    public static void Reset()
    {
        _stats.Clear();
        _slowQueries.Clear();
    }

    /// <summary>
    /// Get a summary report.
    /// </summary>
    public static string GetReport()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== Performance Report ===");
        sb.AppendLine($"Total Operations: {_stats.Values.Sum(s => s.TotalCalls)}");
        sb.AppendLine($"Total Time: {_stats.Values.Sum(s => s.TotalElapsedMs)}ms");
        sb.AppendLine();

        foreach (var (op, stats) in _stats.OrderByDescending(kv => kv.Value.TotalCalls).Take(10))
        {
            var avgMs = stats.TotalCalls > 0 ? stats.TotalElapsedMs / stats.TotalCalls : 0;
            var hitRate = stats.TotalCalls > 0 ? (double)stats.CacheHits / stats.TotalCalls * 100 : 0;
            sb.AppendLine($"{op,-30} Calls: {stats.TotalCalls,6}  Avg: {avgMs,5}ms  Cache Hit: {hitRate,5:F1}%");
        }

        sb.AppendLine();
        sb.AppendLine($"Slow Queries (> {SlowQueryThresholdMs}ms): {_slowQueries.Count}");
        return sb.ToString();
    }

    /// <summary>
    /// Stopwatch helper for timing operations.
    /// </summary>
    public static async Task<T> TimeAsync<T>(string operation, Func<Task<T>> action, bool cacheHit = false)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await action();
        }
        finally
        {
            sw.Stop();
            RecordOperation(operation, sw.ElapsedMilliseconds, cacheHit);
        }
    }

    /// <summary>
    /// Stopwatch helper for timing void operations.
    /// </summary>
    public static async Task TimeAsync(string operation, Func<Task> action, bool cacheHit = false)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await action();
        }
        finally
        {
            sw.Stop();
            RecordOperation(operation, sw.ElapsedMilliseconds, cacheHit);
        }
    }
}

public class OperationStats
{
    public long TotalCalls { get; set; }
    public long TotalElapsedMs { get; set; }
    public long CacheHits { get; set; }

    public double AverageMs => TotalCalls > 0 ? (double)TotalElapsedMs / TotalCalls : 0;
    public double CacheHitRate => TotalCalls > 0 ? (double)CacheHits / TotalCalls * 100 : 0;
}

public class SlowQueryLog
{
    public string Operation { get; set; } = string.Empty;
    public long ElapsedMs { get; set; }
    public DateTime Timestamp { get; set; }
}
