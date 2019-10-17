using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Polly.Caching.Memory.Specs.Integration
{
    public abstract class CacheRoundTripSpecsSyncBase : CacheRoundTripSpecsBase
    {
        protected CacheRoundTripSpecsSyncBase(ICachePolicyFactory cachePolicyFactory) : base(cachePolicyFactory)
        {
        }

        public override Task Should_roundtrip_this_variant_of<TResult>(TResult testValue)
        {
            // Arrange
            var (cacheProvider, cache) = CachePolicyFactory.CreateSyncCachePolicy<TResult, TResult>();

            // Assert - should not be in cache
            (bool cacheHit1, TResult fromCache1) = cacheProvider.TryGet(OperationKey);
            cacheHit1.Should().BeFalse();
            fromCache1.Should().Be(default(TResult));

            // Act - should execute underlying delegate and place in cache
            int underlyingDelegateExecuteCount = 0;
            cache.Execute(ctx =>
                {
                    underlyingDelegateExecuteCount++;
                    return testValue;
                }, new Context(OperationKey))
                .Should().BeEquivalentTo(testValue);

            // Assert - should have executed underlying delegate
            underlyingDelegateExecuteCount.Should().Be(1);

            // Assert - should be in cache
            (bool cacheHit2, TResult fromCache2) = cacheProvider.TryGet(OperationKey);
            cacheHit2.Should().BeTrue();
            fromCache2.Should().BeEquivalentTo(testValue);

            // Act - should execute underlying delegate and place in cache
            cache.Execute(ctx =>
                {
                    underlyingDelegateExecuteCount++;
                    throw new Exception("Cache should be used so this should not get invoked.");
                }, new Context(OperationKey))
                .Should().BeEquivalentTo(testValue);
            underlyingDelegateExecuteCount.Should().Be(1);

            return Task.CompletedTask;
        }
    }
}