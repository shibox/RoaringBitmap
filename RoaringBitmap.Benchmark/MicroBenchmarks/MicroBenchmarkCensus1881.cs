

namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 100)]
    [RankColumn]
    public class MicroBenchmarkCensus1881 : MicroBenchmark
    {
        public MicroBenchmarkCensus1881() : base(DataSets.Census1881)
        {
        }
    }
}