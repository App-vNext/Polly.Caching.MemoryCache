using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Polly.Caching.MemoryCache")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: CLSCompliant(false)] // Because Microsoft.Extensions.Caching.Memory.MemoryCache, on which Polly.Caching.MemoryCache.NetStandard13 depends, is not CLSCompliant.

[assembly: InternalsVisibleTo("Polly.Caching.MemoryCache.NetStandard13.Specs")]