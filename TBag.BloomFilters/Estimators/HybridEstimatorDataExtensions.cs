﻿namespace TBag.BloomFilters.Estimators
{
    using System;

    /// <summary>
    /// Extension methods for the hybrid estimator data.
    /// </summary>
    public static class HybridEstimatorDataExtensions
    {
        /// <summary>
        /// Decode the hybrid estimator data instances.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TId">The type of the entity identifier</typeparam>
        /// <typeparam name="TCount">The type of the occurence count for the Bloom filter.</typeparam>
        /// <param name="estimator">The estimator</param>
        /// <param name="otherEstimatorData">The other estimator</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="destructive">When <c>true</c> the values of <paramref name="estimator"/> will be altered rendering it useless, otherwise <c>false</c></param>
        /// <returns>An estimate of the difference between two sets based upon the estimators.</returns>
        public static long? Decode<TEntity, TId, TCount>(this IHybridEstimatorData<int, TCount> estimator,
            IHybridEstimatorData<int, TCount> otherEstimatorData,
            IBloomFilterConfiguration<TEntity, TId,  int, TCount> configuration,
             bool destructive = false)
            where TCount : struct
            where TId : struct
        {
            if (estimator == null && 
                otherEstimatorData == null) return 0L;
            if (estimator == null ||
                estimator.CountEstimate <= 0L)
                return otherEstimatorData.CountEstimate;
            if (otherEstimatorData == null ||
                otherEstimatorData.CountEstimate <= 0)
                return estimator.CountEstimate;
            var decodeFactor = Math.Max(estimator.StrataEstimator?.DecodeCountFactor ?? 1.0D,
                otherEstimatorData.StrataEstimator?.DecodeCountFactor ?? 1.0D);
            var maxDifference = otherEstimatorData.CountEstimate + estimator.CountEstimate;
            var strataDecode = estimator
                .StrataEstimator
                .Decode(otherEstimatorData.StrataEstimator, configuration, destructive);
            if (!strataDecode.HasValue) return null;
            var minwiseDecode = 2 * (long)(decodeFactor * (estimator.BitMinwiseEstimator.Capacity - 
                     estimator.BitMinwiseEstimator.Similarity(otherEstimatorData.BitMinwiseEstimator) * 
                        estimator.BitMinwiseEstimator.Capacity));
            return strataDecode + minwiseDecode;
        }
    }
}
