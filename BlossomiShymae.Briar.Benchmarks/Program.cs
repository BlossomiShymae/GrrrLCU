using System.Diagnostics;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using BlossomiShymae.Briar.Utils.Behaviors;

namespace BlossomiShymae.Briar.Benchmarks;

public class PortToken
{
    private readonly Process _process;

    public PortToken()
    {
        _process = Process.GetProcessesByName("LeagueClientUx").First();
    }

    [Benchmark]
    public bool WithWin32Native() => new PortTokenWithWin32Native().TryGet(_process, out var _, out var _, out var _);

    [Benchmark]
    public bool WithLockfile() => new PortTokenWithLockfile().TryGet(_process, out var _, out var _, out var _);

}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<PortToken>();
    }
}