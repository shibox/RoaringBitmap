namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkUsCensus2000 : MicroBenchmark
    {
        public MicroBenchmarkUsCensus2000() : base(DataSets.UsCensus2000)
        {
        }
    }
}