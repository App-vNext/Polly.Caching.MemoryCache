namespace Polly.Caching.Memory.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardMemoryCacheProvider_Async : CacheRoundTripSpecsAsyncBase {
        public CacheRoundTripSpecs_NetStandardMemoryCacheProvider_Async() : base(new MemoryCachePolicyFactory())
        {
        }
    }
}