using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Polly.Caching.MemoryCache")]
[assembly: CLSCompliant(false)] // Because Nito.AsycEx, on which Polly.Net40Async depends, is not CLSCompliant.


[assembly: InternalsVisibleTo("Polly.Caching.MemoryCache.Net40Async-Signed.Specs")]