using System;
using Microsoft.Extensions.Caching.Memory;

namespace Polly.Caching.Memory.Specs.Integration
{
    public class MemoryCachePolicyFactory : ICachePolicyFactory
    {
        public (ISyncCacheProvider<TResult>, ISyncPolicy<TResult>) CreateSyncCachePolicy<TCache, TResult>()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            ISyncCacheProvider<TResult> provider = new MemoryCacheProvider(memoryCache).For<TResult>();

            var policy = Policy.Cache<TResult>(provider, TimeSpan.FromHours(1));
            return (provider, policy);
        }

        public (IAsyncCacheProvider<TResult>, IAsyncPolicy<TResult>) CreateAsyncCachePolicy<TCache, TResult>()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            IAsyncCacheProvider<TResult> provider = new MemoryCacheProvider(memoryCache).AsyncFor<TResult>();

            var policy = Policy.CacheAsync<TResult>(provider, TimeSpan.FromHours(1));
            return (provider, policy);
        }
    }
}
