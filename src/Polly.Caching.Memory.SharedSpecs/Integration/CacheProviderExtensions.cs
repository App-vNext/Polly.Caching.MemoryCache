using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching.Memory.Specs.Integration
{
    public static class CacheProviderExtensions
    {
        public static (bool, TResult) TryGet<TResult>(this ISyncCacheProvider<TResult> provider, string key)
        {
            var fromCache = provider.Get(key);
            return (fromCache != null, fromCache);
        }

        public static async Task<(bool, TResult)> TryGetAsync<TResult>(this IAsyncCacheProvider<TResult> provider, string key, CancellationToken token, bool continueOnCapturedContext)
        {
            var fromCache = await provider.GetAsync(key, token, continueOnCapturedContext);
            return (fromCache != null, fromCache);
        }

    }
}
