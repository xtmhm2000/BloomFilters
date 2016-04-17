﻿


namespace TBag.BloomFilters.Estimators
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Decoding algorithm for quasi estimation.
    /// </summary>
    /// <remarks>Quasi estimation uses one Bloom filter (representing the first set) and a set of items (representing the second set) to estimate the number of differences between the two sets.</remarks>
    internal static class QuasiEstimator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="setSize">The total item count for the Bloom filter</param>
        /// <param name="errorRate">Error rate of the Bloom filter</param>
        /// <param name="membershipTest">Test membership in the Bloom filter</param>
        /// <param name="otherSetSample">A set of items to test against the Bloom filter.</param>
        /// <param name="otherSetSize">Optional total size. When not given, the sample size will be used as the total size. When the total size does not match the sample set, the difference will be proportionally scaled.</param>
        /// <param name="membershipCountAdjuster">Optional function to adjust the membership count based upon membership count and sample count (size of <paramref name="otherSetSample"/>.</param>
        /// <returns>The estimated number of differences between the two sets, or <c>null</c> when a reasonable estimate can't be given.</returns>
        /// <remarks>When the <paramref name="otherSetSize"/> is given and does not equal the number of items in <paramref name="otherSetSample"/>, the assumption is that <paramref name="otherSetSample"/> is a random, representative, sample of the total set.</remarks>
        internal static long? Decode<TEntity>(
            long setSize,
            double errorRate,
            Func<TEntity, bool> membershipTest,
            IEnumerable<TEntity> otherSetSample,
            long? otherSetSize = null,
            Func<long, long, long> membershipCountAdjuster = null
            )
        {
            if (otherSetSample == null) return setSize;
            if (setSize == 0L && otherSetSize.HasValue) return otherSetSize.Value;
            var membershipCount = 0L;
            var sampleCount = 0L;
            foreach (var isMember in otherSetSample.Select(membershipTest))
            {
                sampleCount++;
                if (isMember)
                {
                    membershipCount++;
                }
            }
            if (sampleCount == 0) return setSize;
            if (setSize == 0L) return sampleCount;
            if (otherSetSize.HasValue &&
               otherSetSize.Value != sampleCount)
            {
                membershipCount = (long)Math.Ceiling(membershipCount * ((double)otherSetSize.Value / Math.Max(1, sampleCount)));
            }
            if (sampleCount == membershipCount && 
                    setSize != (otherSetSize ?? sampleCount))
            {
                //Obviously there is a difference, but we didn't find one (each item was a member): do the best we can with the set sizes.
                //assume the difference in set size is the major contributor (since we didn't detect many differences in value).
                membershipCount = (otherSetSize ?? sampleCount) == 0L
                    ? 0L
                    : (long)
                        (membershipCount*
                         ((double) Math.Min(setSize, otherSetSize ?? sampleCount)/
                          Math.Max(otherSetSize ?? sampleCount, setSize)));
            }        
            if (membershipCountAdjuster != null)
            {
                membershipCount = membershipCountAdjuster(membershipCount, sampleCount);
            }
            if (membershipCount < 0)
            {
                membershipCount = 0;
            }
            //membership count can't exceed the set size.
            membershipCount = Math.Min(membershipCount, setSize);
            otherSetSize = otherSetSize ?? sampleCount;
            var difference = setSize - otherSetSize.Value +
                             2 * (otherSetSize.Value - membershipCount) / (1 - errorRate);
            //difference is capped by the count of all items.
            return Math.Min((long)Math.Ceiling(Math.Abs(difference)), setSize + otherSetSize.Value);
        }
    }
}