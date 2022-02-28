// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Codefarts.IoC;

BenchmarkRunner.Run<Benchmarks>();

public class Benchmarks
{
    [Benchmark]
    public void Resolve()
    {
        var container = new Container();
        var value= container.Resolve<Stopwatch>();
    }
}