﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBag.BloomFilters.Invertible;
using TBag.BloomFilter.Test.Infrastructure;
using System.Linq;

namespace TBag.BloomFilter.Test.Invertible.Standard
{
    [TestClass]
    public class RemoveTest
    {
        [TestMethod]
        public void InvertibleRemoveItemTest()
        {
            var addSize = 10000;
            var testData = DataGenerator.Generate().Take(addSize).ToArray();
            var errorRate = 0.001F;
            var size = testData.Length;
            var configuration = new DefaultBloomFilterConfiguration();
            var bloomFilter = new InvertibleBloomFilter<TestEntity, long, sbyte>(configuration);
            bloomFilter.Initialize(2*size, errorRate);
            foreach (var itm in testData)
            {
                bloomFilter.Add(itm);
            }
            var contained = testData.Count(item => bloomFilter.Contains(item));
            foreach(var item in testData.Take(addSize / 2))
            {
                bloomFilter.Remove(item);
            }
            var containedAfterRemove = testData.Count(item => bloomFilter.Contains(item));
            //tricky: assuming zero false positives.
            Assert.AreEqual(contained, containedAfterRemove*2, "Wrong item count after removal.");
        }

        [TestMethod]
        public void InvertibleRemoveKeyTest()
        {
            var addSize = 10000;
            var testData = DataGenerator.Generate().Take(addSize).ToArray();
            var errorRate = 0.001F;
            var size = testData.Length;
            var configuration = new DefaultBloomFilterConfiguration();
            var bloomFilter = new InvertibleBloomFilter<TestEntity, long, sbyte>(configuration);
            bloomFilter.Initialize(2 * size, errorRate);
            foreach (var itm in testData)
            {
                bloomFilter.Add(itm);
            }
            var contained = testData.Count(item => bloomFilter.Contains(item));
            foreach (var item in testData.Take(addSize / 2))
            {
                bloomFilter.RemoveKey(item.Id);
            }
            var containedAfterRemove = testData.Count(item => bloomFilter.Contains(item));
            //tricky: assuming zero false positives.
            Assert.AreEqual(contained, containedAfterRemove * 2, "Wrong item count after removal.");
        }
    }
}
