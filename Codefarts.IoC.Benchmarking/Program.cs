// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Codefarts.IoC;
using Codefarts.IoC.Tests;

namespace Codefarts.IoC.Benchmarking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }
}

public class Benchmarks
{
    private const int IterationCount = 10000;
    //    private Container container;
    //
    // [GlobalSetup]
    // public void IterationSetup()
    // {
    //     this.container = new Container();
    // }
    //
    // [GlobalCleanup]
    // public void IterationCleanup()
    // {
    //     this.container = null;
    // }

    [Benchmark]
    public void ResolveStopWatch()
    {
        var container = new Container();
        for (var i = 0; i < IterationCount; i++)
        {
            container.Resolve<Stopwatch>();
        }
    }

    [Benchmark]
    public void ResolveICollectionArrayDependencyWithRegistration()
    {
        var container = new Container();
        container.Register<ICollection<ITestInterface>, ITestInterface[]>();
        for (var i = 0; i < IterationCount; i++)
        {
            container.Resolve<TestClassEnumerableDependency>();
        }
    }

    [Benchmark]
    public void ResolveIEnumerableDependencyWithoutRegistration()
    {
        var container = new Container();
        for (var i = 0; i < IterationCount; i++)
        {
            container.Resolve<TestClassEnumerableDependency>();
        }
    }
}