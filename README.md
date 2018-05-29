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

## Dependencies

Polly.Caching.Memory &gt;= v2.0 works with Polly v6.0.1 and above.

Polly.Caching.MemoryCache &lt;v2.0 works with Polly v5.9.0 and above.

# How to use the Polly.Caching.Memory plugin

```csharp
// (1a): Create a MemoryCacheProvider instance in the .NET Framework, using the Polly.Caching.Memory nuget package.
// (full namespaces and types only shown here for disambiguation)
Polly.Caching.Memory.MemoryCacheProvider memoryCacheProvider 
   = new Polly.Caching.Memory.MemoryCacheProvider(System.Runtime.Caching.MemoryCache.Default);

// Or (1b): Create a MemoryCacheProvider instance in .NET Core / .NET Standard.
// (full namespaces and types only shown here for disambiguation)
// NB Only if you want to create your own Microsoft.Extensions.Caching.Memory.MemoryCache instance:
Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache 
   = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions());
Polly.Caching.Memory.MemoryCacheProvider memoryCacheProvider 
   = new Polly.Caching.Memory.MemoryCacheProvider(memoryCache);

// (2) Create a Polly cache policy using that Polly.Caching.Memory.MemoryCacheProvider instance.
var cachePolicy = Policy.Cache(memoryCacheProvider, TimeSpan.FromMinutes(5));



// Or (1c): Configure by dependency injection within ASP.NET Core
// See https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory
// and https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection#registering-your-own-services

// (In this example we choose to pass a whole PolicyRegistry by dependency injection rather than the individual policy, on the assumption the webapp will probably use multiple policies across the app.)

// For example: 
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<Polly.Caching.IAsyncCacheProvider, Polly.Caching.Memory.MemoryCacheProvider>();

        services.AddSingleton<Polly.Registry.IPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
        {
            PolicyRegistry registry = new PolicyRegistry();
            registry.Add("myCachePolicy", Policy.CacheAsync<HttpResponseMessage>(serviceProvider.GetRequiredService<IAsyncCacheProvider>(), TimeSpan.FromMinutes(5)));
            return registry;
        });

        // ...
    }
}

// In a controller, inject the policyRegistry and retrieve the policy:
// (magic string "myCachePolicy" only hard-coded here to keep the example simple) 
public MyController(IPolicyRegistry<string> policyRegistry)
{
    var _cachePolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("myCachePolicy"); 
    // ...
}

```

For many more configuration options and usage examples of the main Polly `CachePolicy`, see the [main Polly readme](https://github.com/App-vNext/Polly#cache) and [deep doco on the Polly wiki](https://github.com/App-vNext/Polly/wiki/Cache).


# Release notes

For details of changes by release see the [change log](CHANGELOG.md).  


# Acknowledgements

* [@reisenberger](https://github.com/reisenberger) - MemoryCache implementation
* [@seanfarrow](https://github.com/seanfarrow) and [@reisenberger](https://github.com/reisenberger) - Initial caching architecture in the main Polly repo
* [@kesmy](https://github.com/kesmy) - original structuring of the build for msbuild15, in the main Polly repo
* [@seanfarrow](https://github.com/seanfarrow) - v2.0 update to Signed packages only to correspond with Polly v6.0.1


# Instructions for Contributing

Please check out our [Wiki](https://github.com/App-vNext/Polly/wiki/Git-Workflow) for contributing guidelines. We are following the excellent GitHub Flow process, and would like to make sure you have all of the information needed to be a world-class contributor!

Since Polly is part of the .NET Foundation, we ask our contributors to abide by their [Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).

Also, we've stood up a [Slack](http://www.pollytalk.org) channel for easier real-time discussion of ideas and the general direction of Polly as a whole. Be sure to [join the conversation](http://www.pollytalk.org) today!

# License

Licensed under the terms of the [New BSD License](http://opensource.org/licenses/BSD-3-Clause)
