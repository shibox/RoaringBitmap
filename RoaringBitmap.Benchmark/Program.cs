global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Engines;

using BenchmarkDotNet.Running;
using RoaringBitmap.Benchmark;
using RoaringBitmap.Benchmark.MicroBenchmarks;

var types = typeof(MicroBenchmark).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(MicroBenchmark).IsAssignableFrom(t)).ToList();
//var types = new[] { typeof(MicroBenchmarkCensusIncome) };
foreach (var type in types)
{
    //BenchmarkRunner.Run(type);
    BenchMarkUtils.Run(type);
}