﻿using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace RoaringBitmap.Benchmark.MicroBenchmarks
{
    public abstract class MicroBenchmark
    {
        private readonly Collections.Special.RoaringBitmap[] m_Bitmaps;

        protected MicroBenchmark(string fileName)
        {
            var m_Path = @"Data";
            using (var provider = new ZipRealDataProvider(@$"D:\Work\Source\shibox\Repos\RoaringBitmap\RoaringBitmap.Benchmark\bin\Release\net6.0\publish{Path.DirectorySeparatorChar}{m_Path}{Path.DirectorySeparatorChar}{fileName}"))
            //using (var provider = new ZipRealDataProvider(Path.Combine(m_Path, fileName)))
            {
                m_Bitmaps = provider.ToArray();
            }
        }

        [Params(1)]
        public int N;

        [Benchmark]
        public long Or()
        {
            var total = 0L;
            for (var k = 0; k < m_Bitmaps.Length - 1; k++)
            {
                total += (m_Bitmaps[k] | m_Bitmaps[k + 1]).Cardinality;
            }
            return total;
        }

        [Benchmark]
        public long Xor()
        {
            var total = 0L;
            for (var k = 0; k < m_Bitmaps.Length - 1; k++)
            {
                total += (m_Bitmaps[k] ^ m_Bitmaps[k + 1]).Cardinality;
            }
            return total;
        }

        [Benchmark]
        public long And()
        {
            var total = 0L;
            for (var k = 0; k < m_Bitmaps.Length - 1; k++)
            {
                total += (m_Bitmaps[k] & m_Bitmaps[k + 1]).Cardinality;
            }
            return total;
        }

        [Benchmark]
        public long AndNot()
        {
            var total = 0L;
            for (var k = 0; k < m_Bitmaps.Length - 1; k++)
            {
                total += Collections.Special.RoaringBitmap.AndNot(m_Bitmaps[k], m_Bitmaps[k + 1]).Cardinality;
            }
            return total;
        }


        //[Benchmark]
        public long Iterate()
        {
            var total = 0L;
            foreach (var roaringBitmap in m_Bitmaps)
            {
                foreach (var @int in roaringBitmap)
                {
                    unchecked
                    {
                        total += @int;
                    }
                }
            }
            return total;
        }
    }
}