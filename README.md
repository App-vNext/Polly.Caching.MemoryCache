# Polly.Caching.Memory

This repo contains the MemoryCache plugin for the [Polly](https://github.com/App-vNext/Polly) [Cache policy](https://github.com/App-vNext/Polly/wiki/Cache).  The current version targets .NET Standard 1.3 and .NET Standard 2.0.

[![NuGet version](https://badge.fury.io/nu/Polly.Caching.Memory.svg)](https://badge.fury.io/nu/Polly.Caching.Memory) [![Build status](https://ci.appveyor.com/api/projects/status/pgd89nfdr9u4ig8m?svg=true)](https://ci.appveyor.com/project/joelhulen/polly-caching-Memory) [![Slack Status](http://www.pollytalk.org/badge.svg)](http://www.pollytalk.org)

## What is Polly?

[Polly](https://github.com/App-vNext/Polly) is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, Cache aside and Fallback in a fluent and thread-safe manner. Polly targets .NET Standard 1.1 and .NET Standrad 2.0. 

Polly is a member of the [.NET Foundation](https://www.dotnetfoundation.org/about)!

**Keep up to date with new feature announcements, tips & tricks, and other news through [www.thepollyproject.org](http://www.thepollyproject.org)**

![](https://raw.github.com/App-vNext/Polly/master/Polly-Logo.png)

# Installing Polly.Caching.Memory via NuGet

    Install-Package Polly.Caching.Memory


# Supported targets

Polly.Caching.Memory &gt;= v2.0 supports .NET Standard 1.3 and .NET Standard 2.0.

Polly.Caching.MemoryCache &lt;v2.0 supports .NET4.0, .NET4.5 and .NetStandard 1.3

## Versions and Dependencies

Polly.Caching.Memory &gt;=v3.0 requires:

+ [Polly](https://nuget.org/packages/polly) >= v7.0.0.
+ [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/) v2.0.2 or above (or v1.1.2, for NetStandard 1.3).

Polly.Caching.Memory &gt;=v2.0.1 and &lt;v3 requires:

+ [Polly](https://nuget.org/packages/polly) >= v6.1.1 and &lt;v7.
+ [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/) v2.0.2 or above (or v1.1.2, for NetStandard 1.3).

Polly.Caching.Memory v2.0.0 requires:

+ [Polly](https://nuget.org/packages/polly) >= v6.0.1 and &lt;=v6.1.0.
+ [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/) v2.0.2 or above (or v1.1.2, for NetStandard 1.3).

Polly.Caching.MemoryCache v1.* requires:

+ [Polly](https://nuget.org/packages/polly) >=v5.9.0 and &lt;v6.
+ [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory/) v1.1.2, for NetStandard 1.3.

# How to use the Polly.Caching.Memory plugin

### Example: Direct creation of CachePolicy (no DI)

```csharp
// This approach creates a CachePolicy directly, with its own Microsoft.Extensions.Caching.Memory.MemoryCache instance:
Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache 
   = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
Polly.Caching.Memory.MemoryCacheProvider memoryCacheProvider 
   = new Polly.Caching.Memory.MemoryCacheProvider(memoryCache);

// Create a Polly cache policy using that Polly.Caching.Memory.MemoryCacheProvider instance.
var cachePolicy = Policy.Cache(memoryCacheProvider, TimeSpan.FromMinutes(5));
```

### Example: Configure CachePolicy via MemoryCacheProvider in StartUp, for DI

```csharp
// (We pass a whole PolicyRegistry by dependency injection rather than the individual policy, 
// on the assumption the app will probably use multiple policies.)

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<Polly.Caching.IAsyncCacheProvider, Polly.Caching.Memory.MemoryCacheProvider>();

        services.AddSingleton<Polly.Registry.IReadOnlyPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
        {
            PolicyRegistry registry = new PolicyRegistry();
            registry.Add("myCachePolicy", 
                Policy.CacheAsync<HttpResponseMessage>(
                    serviceProvider
                        .GetRequiredService<IAsyncCacheProvider>()
                        .AsyncFor<HttpResponseMessage>(),
                    TimeSpan.FromMinutes(5)));
            return registry;
        });

        // ...
    }
}

// At the point of use, inject the policyRegistry and retrieve the policy:
// (magic string "myCachePolicy" only hard-coded here to keep the example simple) 
public MyController(IReadOnlyPolicyRegistry<string> policyRegistry)
{
    var _cachePolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("myCachePolicy"); 
    // ...
}

```

For many more configuration options and usage examples of the main Polly `CachePolicy`, see the [main Polly readme](https://github.com/App-vNext/Polly#cache) and [deep doco on the Polly wiki](https://github.com/App-vNext/Polly/wiki/Cache).

## Note

`Polly.Caching.Memory.MemoryCacheProvider : ISyncCacheProvider, IAsyncCacheProvider` is non-generic as the underlying `Microsoft.Extensions.Caching.Memory.IMemoryCache` is non-generic.  However, when defining a generic Polly cache policy `Policy.Cache/CacheAsync<TResult>(...)`, some overloads require a generic `ISyncCacheProvider<TResult>` or `IAsyncCacheProvider<TResult>`.  

In this case, use the extensions methods `MemoryCacheProvider.For<TResult>()` or `MemoryCacheProvider.AsyncFor<TResult>()`, as shown in the ASP.NET Core example above, to obtain a generic `ISyncCacheProvider<TResult>` or `IAsyncCacheProvider<TResult>`.

# Release notes

For details of changes by release see the [change log](CHANGELOG.md).  


# Acknowledgements

* [@reisenberger](https://github.com/reisenberger) - MemoryCache implementation
* [@seanfarrow](https://github.com/seanfarrow) and [@reisenberger](https://github.com/reisenberger) - Initial caching architecture in the main Polly repo
* [@kesmy](https://github.com/kesmy) - original structuring of the build for msbuild15, in the main Polly repo
* [@seanfarrow](https://github.com/seanfarrow) - v2.0 update to Signed packages only to correspond with Polly v6.0.1
* [@reisenberger](https://github.com/reisenberger) - Update to Polly v7.0.0


# Instructions for Contributing

Please check out our [Wiki](https://github.com/App-vNext/Polly/wiki/Git-Workflow) for contributing guidelines. We are following the excellent GitHub Flow process, and would like to make sure you have all of the information needed to be a world-class contributor!

Since Polly is part of the .NET Foundation, we ask our contributors to abide by their [Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).

Also, we've stood up a [Slack](http://www.pollytalk.org) channel for easier real-time discussion of ideas and the general direction of Polly as a whole. Be sure to [join the conversation](http://www.pollytalk.org) today!

# License

Licensed under the terms of the [New BSD License](http://opensource.org/licenses/BSD-3-Clause)
