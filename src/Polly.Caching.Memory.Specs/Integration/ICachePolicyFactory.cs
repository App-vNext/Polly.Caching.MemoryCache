namespace Polly.Caching.Memory.Specs.Integration
{
    public interface ICachePolicyFactory
    {
        (ISyncCacheProvider<TResult>, ISyncPolicy<TResult>) CreateSyncCachePolicy<TCache, TResult>();
        (IAsyncCacheProvider<TResult>, IAsyncPolicy<TResult>) CreateAsyncCachePolicy<TCache, TResult>();
    }
}
