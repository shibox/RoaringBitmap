namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkWeatherSept85Srt : MicroBenchmark
    {
        public MicroBenchmarkWeatherSept85Srt() : base(DataSets.WeatherSept85Srt)
        {
        }
    }
}