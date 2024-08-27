using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BlossomiShymae.GrrrLCU.Benchmarks;

public class LcuArguments
{
    private readonly Process _process;

    public LcuArguments()
    {
        _process = Process.GetProcessesByName("LeagueClientUx").First();
    }

    [Benchmark]
    public bool ArgsByWin32Native() => ProcessInfo.TryGetCommandLineArgumentsByWin32Native(_process, out var _, out var _, out var _);

    [Benchmark]
    public bool ArgsByLockfile() => ProcessInfo.TryGetLockfileArguments(_process, out var _, out var _, out var _);

    [Benchmark]
    public bool ArgsByGapotechnko() => ProcessInfo.TryGetCommandLineArgumentsByGapotechnko(_process, out var _, out var _, out var _);

}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<LcuArguments>();
    }
}