# MES Concurrency Stress Test Report

**Date:** 2026-06-18 22:05:20
**Env:** Windows + MySQL + .NET 8.0
**API:** http://localhost:8940

## Summary

| Metric | Value |
|--------|-------|
| Total Tests | 8 |
| Passed | 8 |
| Failed | 0 |
| Avg Response | 72ms |
| Worst P95 | 205ms |

## Results

| Test | Conc | OK | Fail | Rate | Avg(ms) | P95(ms) | Max(ms) |
|------|------|----|------|------|---------|---------|---------|
| WorkOrder List (GET) | 50 | 50 | 0 | 100.0% | 52 | 75 | 76 |
| Lots List (GET) | 50 | 50 | 0 | 100.0% | 72 | 83 | 92 |
| Products (GET) | 50 | 50 | 0 | 100.0% | 52 | 76 | 80 |
| Equipment List (GET) | 50 | 50 | 0 | 100.0% | 48 | 86 | 94 |
| Production Report (GET) | 30 | 30 | 0 | 100.0% | 104 | 117 | 118 |
| Forward Trace (GET) | 30 | 30 | 0 | 100.0% | 40 | 46 | 48 |
| Backward Trace (GET) | 30 | 30 | 0 | 100.0% | 69 | 72 | 73 |
| Create WorkOrder (Write) | 20 | 20 | 0 | 100.0% | 138 | 205 | 205 |

## Rating: EXCELLENT - Production ready

## Recommendations

1. Optimize DB connection pool
2. Add query caching (Redis)
3. Add indexes for complex queries
4. Add optimistic locking for write operations
5. Implement API rate limiting
