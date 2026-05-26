using System.Diagnostics;
using MES.Modules.Production.Models;
using Xunit;
using Xunit.Abstractions;

namespace MES.Modules.Production.Tests;

public class PerformanceBenchmarkTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceBenchmarkTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Benchmark_QuantityValidation_IsFast()
    {
        var sw = Stopwatch.StartNew();
        const int iterations = 10000;
        for (int i = 0; i < iterations; i++)
        {
            var result = new QuantityValidationResult();
            var outputTotal = 90 + 5 + 3 + 2;
            result.ExpectedTotal = 100;
            result.ActualTotal = outputTotal;
            if (outputTotal != 100)
                result.AddError("Unbalanced");
        }
        sw.Stop();

        _output.WriteLine($"{iterations} validations: {sw.ElapsedMilliseconds}ms");
        Assert.True(sw.ElapsedMilliseconds < 1000, $"Should complete within 1s, took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void Benchmark_ModelCreation_IsFast()
    {
        var sw = Stopwatch.StartNew();
        const int iterations = 10000;
        for (int i = 0; i < iterations; i++)
        {
            var lot = new LotInfo
            {
                LotId = $"LOT-{i}",
                Status = "Processing",
                RouteId = "RT-001",
                CurrentStepSeq = 1,
            };
        }
        sw.Stop();

        _output.WriteLine($"{iterations} model creations: {sw.ElapsedMilliseconds}ms");
        Assert.True(sw.ElapsedMilliseconds < 1000, $"Should complete within 1s, took {sw.ElapsedMilliseconds}ms");
    }
}
