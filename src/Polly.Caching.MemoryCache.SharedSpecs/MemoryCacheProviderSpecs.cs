using System;
using System.Threading;
using FluentAssertions;
using Polly.Caching;
using Xunit;
using Polly.Caching.MemoryCache;

#if PORTABLE
using MemoryCacheImplementation = Microsoft.Extensions.Caching.Memory.IMemoryCache;
#else
using MemoryCacheImplementation = System.Runtime.Caching.MemoryCache;
#endif

namespace Polly.Specs.Caching.MemoryCache
{
    public class MemoryCacheProviderSpecs
    {
        #region Configuration

        [Fact]
        public void Should_throw_when_MemoryCacheImplementation_is_null()
        {
           Action configure = () => new MemoryCacheProvider(null);

            configure.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("memoryCache");

        }

        [Fact]
        public void Should_not_throw_when_MemoryCacheImplementation_is_not_null()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            Action configure = () => new MemoryCacheProvider(memoryCache);

            configure.ShouldNotThrow();
        }

        #endregion

        #region Get

        [Fact]
        public void Get_should_return_instance_previously_stored_in_cache()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();
#if PORTABLE
            using (Microsoft.Extensions.Caching.Memory.ICacheEntry entry = memoryCache.CreateEntry(key)) { 
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                entry.Value = value;
            }
#else
            memoryCache[key] = value;
#endif

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            object got = provider.Get(key);
            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Get_should_return_null_on_unknown_key()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            object got = provider.Get(Guid.NewGuid().ToString());
            got.Should().BeNull();
        }

        #endregion

        #region Put

        [Fact]
        public void Put_should_put_item_into_configured_MemoryCacheImplementation()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(TimeSpan.FromSeconds(10));
            provider.Put(key, value, ttl);

#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif

            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            TimeSpan shimTimeSpan = TimeSpan.FromSeconds(0.1); // If test fails transiently in different environments, consider increasing shimTimeSpan.

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(shimTimeSpan, false);
            provider.Put(key, value, ttl);

            // Initially (before ttl expires), should be able to get value from cache.
#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif
            got.Should().BeSameAs(value);

            // Wait until the TTL on the cache item should have expired.
            Thread.Sleep(shimTimeSpan + shimTimeSpan);

#if PORTABLE
            memoryCache.TryGetValue(key, out got);
#else
            got = memoryCache[key];
#endif

            got.Should().NotBeSameAs(value);
            got.Should().BeNull();
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            TimeSpan shimTimeSpan = TimeSpan.FromSeconds(1); // If test fails transiently in different environments, consider increasing shimTimeSpan.

            string key = "anything";
            object value = new object();

            // Place an item in the cache that should last for only 2x shimTimespan
            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(shimTimeSpan + shimTimeSpan, true);
            provider.Put(key, value, ttl);

            // Prove that we can repeatedly get it from the cache over a 5x shimTimespan period, due to repeated access.

            for (int i = 0; i < 5; i++)
            {
#if PORTABLE
                object got;
                memoryCache.TryGetValue(key, out got);
#else
                object got = memoryCache[key];
#endif

                got.Should().BeSameAs(value, $"at iteration {i}");

                Thread.Sleep(shimTimeSpan);
            }
        }

        #region Boundary tests

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl_maxvalue()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(TimeSpan.MaxValue, false);
            provider.Put(key, value, ttl);

#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif
            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl_maxvalue()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan maxSlidingExpiration =
#if PORTABLE
            TimeSpan.MaxValue
#else
            TimeSpan.FromDays(365) // This is the maximum permitted sliding ttl for .NetFramework4.0 and 4.5 MemoryCache.
#endif       
            ;

            Ttl ttl = new Ttl(maxSlidingExpiration, true);
            provider.Put(key, value, ttl);

#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif
            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl_zero()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan minExpiration =
#if PORTABLE
            TimeSpan.FromMilliseconds(1)  // This is the minimum permitted non-sliding ttl for .NetStandard
#else
            TimeSpan.Zero
#endif
            ;

            Ttl ttl = new Ttl(minExpiration, false);
            provider.Put(key, value, ttl);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif
            got.Should().BeNull();
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl_zero()
        {
#if PORTABLE
            MemoryCacheImplementation memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
#else
            MemoryCacheImplementation memoryCache = System.Runtime.Caching.MemoryCache.Default;
#endif

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan minExpiration =
#if PORTABLE
            TimeSpan.FromMilliseconds(1)  // This is the minimum permitted sliding ttl for .NetStandard
#else
            TimeSpan.Zero
#endif
            ;

            Ttl ttl = new Ttl(minExpiration, false);
            provider.Put(key, value, ttl);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

#if PORTABLE
            object got;
            memoryCache.TryGetValue(key, out got);
#else
            object got = memoryCache[key];
#endif
            got.Should().BeNull();
        }
        #endregion

        #endregion
    }
}