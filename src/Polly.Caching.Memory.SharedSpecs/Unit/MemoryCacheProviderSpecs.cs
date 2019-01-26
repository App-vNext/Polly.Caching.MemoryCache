using System;
using System.Threading;
using FluentAssertions;
using Polly.Caching;
using Xunit;
using Polly.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;

namespace Polly.Specs.Caching.Memory.Unit
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
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            Action configure = () => new MemoryCacheProvider(memoryCache);

            configure.ShouldNotThrow();
        }

        #endregion

        #region Get

        [Fact]
        public void Get_should_return_instance_previously_stored_in_cache()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = Guid.NewGuid().ToString();
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
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            object got = provider.Get(Guid.NewGuid().ToString());
            got.Should().BeNull();
        }

        #endregion

        #region Put

        [Fact]
        public void Put_should_put_item_into_configured_MemoryCacheImplementation()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = Guid.NewGuid().ToString();
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(TimeSpan.FromSeconds(10));
            provider.Put(key, value, ttl);

            object got;
            memoryCache.TryGetValue(key, out got);

            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            TimeSpan shimTimeSpan = TimeSpan.FromSeconds(0.1); // If test fails transiently in different environments, consider increasing shimTimeSpan.

            string key = Guid.NewGuid().ToString();
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(shimTimeSpan, false);
            provider.Put(key, value, ttl);

            // Initially (before ttl expires), should be able to get value from cache.
            object got;
            memoryCache.TryGetValue(key, out got);
            got.Should().BeSameAs(value);

            // Wait until the TTL on the cache item should have expired.
            Thread.Sleep(shimTimeSpan + shimTimeSpan);

            memoryCache.TryGetValue(key, out got);

            got.Should().NotBeSameAs(value);
            got.Should().BeNull();
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            TimeSpan shimTimeSpan = TimeSpan.FromSeconds(1); // If test fails transiently in different environments, consider increasing shimTimeSpan.

            string key = Guid.NewGuid().ToString();
            object value = new object();

            // Place an item in the cache that should last for only 2x shimTimespan
            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(shimTimeSpan + shimTimeSpan, true);
            provider.Put(key, value, ttl);

            // Prove that we can repeatedly get it from the cache over a 5x shimTimespan period, due to repeated access.

            for (int i = 0; i < 5; i++)
            {
                object got;
                memoryCache.TryGetValue(key, out got);

                got.Should().BeSameAs(value, $"at iteration {i}");

                Thread.Sleep(shimTimeSpan);
            }
        }

        #region Boundary tests

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl_maxvalue()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);
            Ttl ttl = new Ttl(TimeSpan.MaxValue, false);
            provider.Put(key, value, ttl);

            object got;
            memoryCache.TryGetValue(key, out got);
            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl_maxvalue()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan maxSlidingExpiration = TimeSpan.MaxValue;
            
            Ttl ttl = new Ttl(maxSlidingExpiration, true);
            provider.Put(key, value, ttl);

            object got;
            memoryCache.TryGetValue(key, out got);
            got.Should().BeSameAs(value);
        }

        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl_zero()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan minExpiration = TimeSpan.FromMilliseconds(1);  // This is the minimum permitted non-sliding ttl for .NetStandard

            Ttl ttl = new Ttl(minExpiration, false);
            provider.Put(key, value, ttl);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            object got;
            memoryCache.TryGetValue(key, out got);
            got.Should().BeNull();
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl_zero()
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

            string key = "anything";
            object value = new object();

            MemoryCacheProvider provider = new MemoryCacheProvider(memoryCache);

            TimeSpan minExpiration = TimeSpan.FromMilliseconds(1);  // This is the minimum permitted sliding ttl for .NetStandard

            Ttl ttl = new Ttl(minExpiration, false);
            provider.Put(key, value, ttl);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            object got;
            memoryCache.TryGetValue(key, out got);
            got.Should().BeNull();
        }
        #endregion

        #endregion
    }
}