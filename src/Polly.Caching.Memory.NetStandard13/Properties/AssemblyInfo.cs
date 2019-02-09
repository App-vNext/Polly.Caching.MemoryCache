using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Polly.Caching.Memory")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
[assembly: AssemblyInformationalVersion("3.0.0.0")]
[assembly: CLSCompliant(false)] // Because Microsoft.Extensions.Caching.Memory.MemoryCache, on which Polly.Caching.MemoryCache.NetStandard13 depends, is not CLSCompliant.

[assembly: InternalsVisibleTo("Polly.Caching.Memory.NetStandard13.Specs")]