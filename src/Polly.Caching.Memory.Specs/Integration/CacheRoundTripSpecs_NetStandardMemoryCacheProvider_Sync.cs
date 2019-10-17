namespace Polly.Caching.Memory.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardMemoryCacheProvider_Sync : CacheRoundTripSpecsSyncBase {
        public CacheRoundTripSpecs_NetStandardMemoryCacheProvider_Sync() : base(new MemoryCachePolicyFactory())
        {
        }
    }
}