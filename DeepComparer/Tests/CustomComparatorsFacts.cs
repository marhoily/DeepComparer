using System;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class CustomComparatorsFacts
    {
        private readonly DeepComparerBuilder _comparer =
            new DeepComparerBuilder();
        public class X
        {
            public int I { get; set; }
        }
        [Fact]
        public void Custom_False()
        {
            _comparer
                .DelveInto(t => t == typeof(X))
                .RuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Build()
                .Compare(new X { I = 3 }, new X { I = 4 }, typeof(X))
                .Should().BeTrue();
        }
        [Fact]
        public void Custom_True()
        {
            _comparer
                .DelveInto(t => t == typeof(X))
                .RuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Build()
                .Compare(new X { I = 3 }, new X { I = 40 }, typeof(X))
                .Should().BeFalse();
        }

    }
}