﻿using System.Reflection;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class ComparerFacts
    {
        private readonly DeepComparerBuilder _builder =
            new DeepComparerBuilder();

        [Fact]
        public void PassNull()
        {
            var notNull = new X { I = 3 };
            _builder.Build().Compare(null, notNull).Should().BeFalse();
            _builder.Build().Compare<X>(null, null).Should().BeTrue();
        }
        [Fact]
        public void Do_Not_Expand_Objects_By_Default()
        {
            _builder.Build()
                .Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void ExpandObject_Example()
        {
            _builder
                .GoDeepFor(t => t == typeof(X))
                .Build()
                .Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_Throw()
        {
            _builder.GoDeepFor(t => t == typeof(X));
            Assert.Throws<TargetException>(()
                => _builder.Build().Compare(new X(), new Y(), typeof(X)));
        }

        public class X { public int I { get; set; } }
        public class Y { public int I { get; set; } }
    }
}