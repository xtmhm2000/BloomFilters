﻿namespace TBag.BloomFilters.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Count configuration
    /// </summary>
    public class IntCountConfiguration : CountConfigurationBase<int>
    {
        /// <summary>
        /// Decrease the count
        /// </summary>
        public override Func<int, int> Decrease { get; set; } = DecreaseImpl;

        private static int DecreaseImpl(int c)
        {
            try
            {
                return checked(c - 1);
            }
            catch (OverflowException)
            {
                return int.MinValue;
            }
        }

        /// <summary>
        /// The count identity value (0)
        /// </summary>
        public override Func<int> Identity { get; set; } = ()=>0;

        /// <summary>
        /// Increase the count
        /// </summary>
        public override Func<int, int> Increase { get; set; } = IncreaseImpl;

        private static int IncreaseImpl(int c)
        {
            try
            {
                return checked(c + 1);
            }
            catch (OverflowException)
            {
                return int.MaxValue;
            }
        }

        /// <summary>
        /// Subtract counts
        /// </summary>
        public override Func<int, int, int> Subtract { get; set; } = SubtractImpl;

        private static int SubtractImpl(int c1, int c2)
        {
            try
            {
                return checked(c1 - c2);
            }
            catch (OverflowException)
            {
                return c2 > 0 ? int.MinValue : int.MaxValue;
            }
        }

        /// <summary>
        /// The count unity (1)
        /// </summary>
        public override Func<int> Unity { get; set; } = ()=>1;

        /// <summary>
        /// Count comparer
        /// </summary>
        public override IComparer<int> Comparer { get; set; } = Comparer<int>.Default;

        /// <summary>
        /// Determine if the count is pure.
        /// </summary>
        public override Func<int, bool> IsPure { get; set; } = IsPureImpl;

        private static bool IsPureImpl(int c)
        {
            try
            {
                return checked(Math.Abs(c) == 1);
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        /// <summary>
        /// Add two counts
        /// </summary>
        public override Func<int, int, int> Add { get; set; } = AddImpl;

        private static int AddImpl(int c1, int c2)
        {
            try
            {
                return checked(c1 + c2);
            }
            catch (OverflowException)
            {
                return c2 > 0 ? int.MaxValue : int.MinValue;
            }
        }

        /// <summary>
        /// Determine if given the size of the Bloom filter, this count configuration is expected to be able to support the capacity.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public override bool Supports(long capacity, long size)
        {
            return (int.MaxValue - 30) * size > capacity;
        }

        /// <summary>
        /// Get the estimated number of items in the Bloom filter.
        /// </summary>
        /// <param name="counts"></param>
        /// <param name="hashSize"></param>
        /// <returns></returns>
        public override long GetEstimatedCount(int[] counts, uint hashSize)
        {
            if (counts == null || hashSize <= 0) return 0L;
            return counts.Select(c => (long)c).Sum(c=>Math.Abs(c)) / hashSize;
        }
    }
}
