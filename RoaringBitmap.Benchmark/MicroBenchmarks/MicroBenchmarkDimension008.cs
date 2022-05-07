﻿namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 1)]
    [RankColumn]
    public class MicroBenchmarkDimension008 : MicroBenchmark
    {
        public MicroBenchmarkDimension008() : base(DataSets.Dimension008)
        {
        }
    }
}