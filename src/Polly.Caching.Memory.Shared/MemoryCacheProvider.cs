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
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>
        /// A tuple whose first element is a value indicating whether the key was found in the cache,
        /// and whose second element is the value from the cache (null if not found).
        /// </returns>
        public (bool, object) TryGet(string key)
        {
            bool cacheHit = _cache.TryGetValue(key, out var value);
            return (cacheHit, value);
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
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

            if (ttl.SlidingExpiration)
            {
                options.SlidingExpiration = ttl.Timespan < remaining ? ttl.Timespan : remaining;
            }
            else
            {
                if (ttl.Timespan == TimeSpan.MaxValue)
                {
                    options.AbsoluteExpiration = DateTimeOffset.MaxValue;
                }
                else
                {
                    options.AbsoluteExpirationRelativeToNow = ttl.Timespan < remaining ? ttl.Timespan : remaining;
                }
            }

            _cache.Set(key, value, options);
        }

        /// <summary>
        /// Gets a value from the cache asynchronously.
        /// <para><remarks>The implementation is synchronous as there is no advantage to an asynchronous implementation for an in-memory cache.</remarks></para>
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>Note: if the underlying cache's async API does not support controlling whether to continue on a captured context, async Policy executions with continueOnCapturedContext == true cannot be guaranteed to remain on the captured context.</remarks></para></param>
        /// <returns>
        /// A <see cref="Task{TResult}" /> promising as Result a tuple whose first element is a value indicating whether
        /// the key was found in the cache, and whose second element is the value from the cache (null if not found).
        /// </returns>
        public Task<(bool, object)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(TryGet(key));
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
            return Task.CompletedTask;
        }
    }
}
