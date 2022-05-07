namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkCensus1881Srt : MicroBenchmark
    {
        public MicroBenchmarkCensus1881Srt() : base(DataSets.Census1881Srt)
        {
        }
    }
}