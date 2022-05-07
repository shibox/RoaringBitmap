namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkDimension003 : MicroBenchmark
    {
        public MicroBenchmarkDimension003() : base(DataSets.Dimension003)
        {
        }
    }
}