using System;
using Polly.Utilities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Polly.Caching.Memory
{
    /// <summary>
    /// A cache provider for the Polly CachePolicy, using a passed-in instance of <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/> as the store.
    /// </summary>
    public class MemoryCacheProvider : ISyncCacheProvider, IAsyncCacheProvider
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheProvider"/> class.
        /// </summary>
        /// <param name="memoryCache">The memory cache instance in which to store cached items.</param>
        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            if (memoryCache == null) throw new ArgumentNullException(nameof(memoryCache));
            _cache = memoryCache;
        }

        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The value from cache; or null, if none was found.</returns>
        public object Get(String key)
        {
            object value;
            if (_cache.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }
        
        /// <summary>
        /// Puts the specified value in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        public void Put(string key, object value, Ttl ttl)
        {
            TimeSpan remaining = DateTimeOffset.MaxValue - SystemClock.DateTimeOffsetUtcNow();

            using (Microsoft.Extensions.Caching.Memory.ICacheEntry entry = _cache.CreateEntry(key))
            {
                entry.Value = value;
                if (ttl.SlidingExpiration)
                {
                    entry.SlidingExpiration = ttl.Timespan < remaining ? ttl.Timespan : remaining;
                }
                else
                {
                    if (ttl.Timespan ==TimeSpan.MaxValue)
                    {
                        entry.AbsoluteExpiration = DateTimeOffset.MaxValue;
                    }
                    else
                    {
                        entry.AbsoluteExpirationRelativeToNow = ttl.Timespan < remaining ? ttl.Timespan : remaining;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value from the memory cache as part of an asynchronous execution.  <para><remarks>The implementation is synchronous as there is no advantage to an asynchronous implementation for an in-memory cache.</remarks></para>
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.  </param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="MemoryCacheProvider"/>, this parameter is irrelevant and is ignored, as the implementation is synchronous.</remarks></para></param>
        /// <returns>A <see cref="Task{TResult}" /> promising as Result the value from cache; or null, if none was found.</returns>
        public Task<object> GetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return TaskHelper.FromResult(Get(key));
            // (With C#7.0, a ValueTask<> approach would be preferred, but some of our tfms do not support that.  TO DO: Implement it, with preprocessor if/endif directives, for NetStandard)
        }

        /// <summary>
        /// Puts the specified value in the cache as part of an asynchronous execution.
        /// <para><remarks>The implementation is synchronous as there is no advantage to an asynchronous implementation for an in-memory cache.</remarks></para>
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="MemoryCacheProvider"/>, this parameter is irrelevant and is ignored, as the implementation is synchronous.</remarks></para></param>
        /// <returns>A <see cref="Task" /> which completes when the value has been cached.</returns>
        public Task PutAsync(string key, object value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Put(key, value, ttl);
            return TaskHelper.EmptyTask;
            // (With C#7.0, a ValueTask<> approach would be preferred, but some of our tfms do not support that. TO DO: Implement it, with preprocessor if/endif directives, for NetStandard)
        }
    }
}
