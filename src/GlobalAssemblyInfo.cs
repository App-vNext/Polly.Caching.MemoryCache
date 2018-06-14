using System.Reflection;

[assembly: AssemblyProduct("Polly.Caching.Memory")]
[assembly: AssemblyCompany("App vNext")]
[assembly: AssemblyDescription("Polly.Caching.Memory is a MemoryCache plug-in for the Polly CachePolicy.  Polly is a library that allows developers to express resilience and transient fault handling policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.")]
[assembly: AssemblyCopyright("Copyright (c) 2018, App vNext")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
