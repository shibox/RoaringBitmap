namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkWeatherSept85 : MicroBenchmark
    {
        public MicroBenchmarkWeatherSept85() : base(DataSets.WeatherSept85)
        {
        }
    }
}