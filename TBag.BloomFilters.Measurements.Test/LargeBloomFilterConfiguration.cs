﻿namespace TBag.BloomFilters.Measurements.Test
{
    using System;
    using System.Text;
    using BloomFilters;
    using HashAlgorithms;

    /// <summary>
    /// A test Bloom filter configuration.
    /// </summary>
    internal class LargeBloomFilterConfiguration : StandardIbfConfigurationBase<TestEntity, int>
    {
        public LargeBloomFilterConfiguration() : base(new HighUtilizationCountConfiguration())
        {}

        protected override long GetIdImpl(TestEntity entity)
        {
            return entity?.Id ?? 0L;
        }
    }
}