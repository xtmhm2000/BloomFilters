﻿namespace TBag.BloomFilters.Estimators
{
    using System;

    /// <summary>
    /// Encapsulates emperical data for creating hybrid estimators.
    /// </summary>
    public class HybridEstimatorFactory : IHybridEstimatorFactory
    {
        /// <summary>
        /// Create a hybrid estimator
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <typeparam name="TId">The type of the entity identifier</typeparam>
        /// <typeparam name="TCount">The type of occurence count.</typeparam>
        /// <param name="configuration">Bloom filter configuration</param>
        /// <param name="setSize">Number of elements in the set that is added.</param>
        /// <param name="failedDecodeCount">Number of times decoding has failed based upon the provided estimator.</param>
        /// <returns></returns>
        public IHybridEstimator<TEntity, int, TCount> Create<TEntity, TId, TCount>(
            IBloomFilterConfiguration<TEntity,  TId,  int, TCount> configuration,
            long setSize,
            byte failedDecodeCount = 0)
            where TCount : struct
            where TId : struct
        {
            byte strata = 7;
            var capacity = (long)(50 * Math.Max(1.0D, Math.Log10(setSize)));
            byte bitSize = 2;
            var minwiseHashCount = 8;
            if (setSize > 8000L)
            {
                strata = 9;
                minwiseHashCount = 10;
            }
            else if (setSize > 16000L)
            {
                strata = 13;
                if (capacity < 1000)
                {
                    capacity = 1000;
                }
                minwiseHashCount = 15;
            }
            if (failedDecodeCount >= 1)
            {
                strata = (byte)(setSize > 10000L || failedDecodeCount > 1 
                    ? 13
                    : 9);
            }
            if (failedDecodeCount > 1 &&
                capacity < (long)0.2D * setSize)
            {
                capacity = failedDecodeCount < 2
                    ? (long)0.2D * setSize
                    : (long)0.5D * setSize;
            }
            var result = new HybridEstimator<TEntity, TId, TCount>(
                capacity,
                bitSize,
                minwiseHashCount,
                setSize,
                strata,
                configuration)
            { };
            if (failedDecodeCount > 1)
            {
                result.DecodeCountFactor = Math.Pow(2, failedDecodeCount);
            }
            return result;
        }

        /// <summary>
        /// Create an estimator that matches the given <paramref name="data"/> estimator.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity</typeparam>
        /// <typeparam name="TId">The type of the identifier</typeparam>
        /// <typeparam name="TCount">The type of the occurence count</typeparam>
        /// <param name="data">The estimator data to match</param>
        /// <param name="configuration">The Bloom filter configuration</param>
        /// <param name="setSize">The (estimated) size of the set to add to the estimator.</param>
        /// <returns>An estimator</returns>
        public IHybridEstimator<TEntity, int, TCount> CreateMatchingEstimator<TEntity, TId, TCount>(
            IHybridEstimatorData<int, TCount> data,
            IBloomFilterConfiguration<TEntity, TId, int,  TCount> configuration,
            long setSize)
            where TCount : struct
            where TId : struct
        {
            return new HybridEstimator<TEntity, TId, TCount>(
                data.Capacity,
                data.BitMinwiseEstimator.BitSize,
                data.BitMinwiseEstimator.HashCount,
                setSize,
                data.StrataCount,
                configuration)
            {
                DecodeCountFactor = data.StrataEstimator.DecodeCountFactor
            };
        }
    }
}
