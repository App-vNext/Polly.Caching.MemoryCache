# Polly.Caching.Memory change log

## 3.0.2
- No functional changes
- Updated Polly dependency to latest, v7.1.1
- Consolidated solution and fixed build
- Added NetStandard 2.1 target (for .NET Core3.0 consumption)
- Added test runs in netcoreapp3.0; .NET Framework 4.6.1; and .NET Framework 4.7.2
- Updated FluentAssertions and xUnit dependencies
- Updated NetStandard2.0 dependency to avoid vulnerability for Net Standard 2.0 consumption target in underlying Microsoft.Extensions.Caching.Memory package. Detail:  https://securitytracker.com/id/1040152 ;  https://github.com/App-vNext/Polly.Caching.MemoryCache/issues/37.

_Note:_ This vulnerability was in the underlying Microsoft.Extensions.Caching.Memory package, not in Polly.Caching.MemoryCache.  Consumers should have been aware of this vulnerabilty via their normal Microsoft vulnerability alert mechanisms, and updating Microsoft.Extensions.Caching.Memory in their solutions would have caused Polly.Caching.MemoryCache also to reference the updated version.  However, this update patches Polly.Caching.Memory such that it is not possible to install the latest version v3.0.2 for the .Net Standard 2.0 target and reference a vulnerable underlying version of Microsoft.Extensions.Caching.Memory.

## 3.0.1
- No functional changes
- Updated Polly dependency to &gt;> v7.0.2, which includes a bug fix for PolicyRegistry

## 3.0.0
- Allow caching of `default(TResult)`
- Compatible with Polly &gt;= v7

## 2.0.2
- No functional changes
- Indicate compatibility with Polly &lt; v7

## 2.0.2
- No functional changes
- Indicate compatibility with Polly &lt; v7

## 2.0.1
- Upgrade for compatibility with Polly v6.1.1

## 2.0.0
- Provide a single signed package only.
- Reference Polly v6.0.1.
- Remove .net 4 and 4.5 support. 
- Add .net standard 2.0 as a target framework.
- Change namespaces from Polly.Caching.MemoryCache to Polly.Caching.Memory to avoid clashes.

## 1.1.0

- Polly.Caching.MemoryCache-Signed now references Polly-Signed 
- Reference Polly v5.9.0 to bring in cache fixes

## 1.0-RC

- Upgrade to Polly v5.4.0
- Correctly state RC dependency (Polly v5.4.0)

## 0.2-beta

- Beta implementation
- Rebase against Polly v5.3.x
- Upgrade to msbuild15 build process 

## 0.1-alpha

- Stub repo for Polly.Caching.MemoryCache with first build script
