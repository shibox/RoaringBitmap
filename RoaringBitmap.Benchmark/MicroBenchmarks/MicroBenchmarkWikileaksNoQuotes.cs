namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkWikileaksNoQuotes : MicroBenchmark
    {
        public MicroBenchmarkWikileaksNoQuotes() : base(DataSets.WikileaksNoQuotes)
        {
        }
    }
}