# Invertible Bloom Filters
On using invertible Bloom filters for estimating differences between sets of key/value pairs. Written in C#.

The goal is to efficiently determine the differences between two sets of key/value pairs. 

The first approach is based upon invertible Bloom filters, as described in "What’s the Difference? Efﬁcient Set Reconciliation without Prior Context" (David Eppstein, Michael T. Goodrich, Frank Uyeda, George Varghese, 2011, http://conferences.sigcomm.org/sigcomm/2011/papers/sigcomm/p218.pdf) . A similar data structure, but not extended to detect set differences, is described in "Invertible Bloom Lookup Tables" (Michael T. Goodrich, Michael Mitzenmacher, 2015, http://arxiv.org/pdf/1101.2245v3.pdf). In this paper an invertible Bloom filter is presented with a subtraction operator that determines the difference between two Bloom filters. After implementing the data structure as described in the paper, it was noted that the data structure detected changes in the keys, but did not detect changes in the values. The following additions were made to account for changes in the values as well:

1. Any pure items that after substraction have count zero, but do not have zero for the value hash, will have their Id added to the set of differences. Intuitively this means any item stored by itself in the same location across both Bloom filters, has a different value when the hash of the values is different.
2. During decoding, any of the locations finally ending up with count equal to zero, will be evaluated for non zero identifier hashes or non zero value hashes. Intuitively this means that during decoding we can identify difference not only from pure locations, but also from locations that transition from pure to zero.

Included with the Bloom Filter is a strata estimator (as described in the above paper). Based upon the strata estimator, a hybrid strata estimator was implemented utilizing the b-bit minwise hash (described in http://research.microsoft.com/pubs/120078/wfc0398-liPS.pdf). 

The estimator is important, because an estimate of the number of differences is needed to pick a proper sized Bloom filter that can be decoded. The size of the invertible Bloom filter needed for detecting the changes between two sets is not dependent upon the set sizes, but upon the size of the difference.  For example, 30 differences between two sets of 500000 elements each can be fully detected by 63 kilobyte invertible Bloom filter. On the other hand, 40000 differences between two sets of 45000 items each can take a Bloom filter of 17 megabytes. 

When the estimate for the difference is too large, a Bloom filter will be used that requires more space than needed. When the estimate is too small, a Bloom filter might be used that can't be successfully decoded, additional space and time is required to find a Bloom filter that is large enough to be succesfully decoded.

## Serialization
Support has been added for serializing and deserializing Bloom filters and estimators.

## Overloading a Bloom filter
When utilizing an invertible Bloom filter within the capacity it was sized for, the count will seldom exceed 2 or 3. However, when utilizing estimators, the idea is that the invertible Bloom filter will be utilized at a much higher capacity than it was sized for, thus accepting a higher error rate and much higher count values. To account for both scenario's, the actual count type is configurable. Two types will be supported out of the box: sbyte and int.


## Wishlist
Although this is initially just a testbed, an obvious wishlist item is a buffer pool to counteract some of the horrible things the Bloom Filter does to memory management.

