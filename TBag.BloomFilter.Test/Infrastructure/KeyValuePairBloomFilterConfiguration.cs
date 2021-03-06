﻿namespace TBag.BloomFilter.Test.Infrastructure
{
    using BloomFilters.Countable.Configurations;
    using System;
    using TBag.BloomFilters;
    using TBag.BloomFilters.Configurations;
    using TBag.BloomFilters.Invertible.Configurations;

    internal class KeyValuePairBloomFilterConfiguration : PairConfigurationBase<sbyte>
    {       

        public KeyValuePairBloomFilterConfiguration(ICountConfiguration<sbyte> configuration, bool createValueFilter = true) : 
            base(configuration, createValueFilter)
        {
        }

        public override IFoldingStrategy FoldingStrategy { get; set; } = new SmoothNumbersFoldingStrategy();
    }
}
