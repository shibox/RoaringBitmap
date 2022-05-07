﻿namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkDimension033 : MicroBenchmark
    {
        public MicroBenchmarkDimension033() : base(DataSets.Dimension033)
        {
        }
    }
}