﻿namespace TBag.BloomFilter.Test
{
    using System;
    using System.Text;
    using HashAlgorithms;
    using BloomFilters.Configurations;
    using BloomFilters.Invertible.Configurations;    /// <summary>
                                                     /// A test Bloom filter configuration.
                                                     /// </summary>
    internal class KeyValueLargeBloomFilterConfiguration : ReverseConfigurationBase<TestEntity, short>
    {
        private readonly IMurmurHash _murmurHash = new Murmur3();

        public KeyValueLargeBloomFilterConfiguration() : base(new ShortCountConfiguration())
        {}

        protected override long GetIdImpl(TestEntity entity)
        {
            return entity?.Id ?? 0L;
        }

        protected override int GetEntityHashImpl(TestEntity entity)
        {
            return BitConverter.ToInt32(_murmurHash.Hash(Encoding.UTF32.GetBytes(entity.Value)), 0);
        }

        public override IFoldingStrategy FoldingStrategy { get; set; } = new SmoothNumbersFoldingStrategy();
    }
}