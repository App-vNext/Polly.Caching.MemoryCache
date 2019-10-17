# Polly.Caching.Memory change log

## 3.0.2
- No functional changes
- Updated NetStandard2.0 dependency to avoid vulnerability in underlying Microsoft.Extensions.Caching.Memory package. Updated dependency to v2.2.0, to avoid vulnerability https://cve.mitre.org/cgi-bin/cvename.cgi?name=CVE-2018-0786. See https://github.com/App-vNext/Polly.Caching.MemoryCache/issues/37 for more details.
- Updated Polly dependency to latest, v7.1.1

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
