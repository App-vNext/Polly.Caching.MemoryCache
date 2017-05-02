using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;
using Polly.Caching.MemoryCache;

namespace Polly.Caching.MemoryCache.Specs
{
    public class PolicySpecs
    {
        
        [Fact]
        public void SomeTest()
        {
            SomeClass.SomeMethod().Should().BeTrue();
        }

        
    }
}