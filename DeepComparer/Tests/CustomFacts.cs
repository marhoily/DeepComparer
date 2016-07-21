using System;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class CustomFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X))
                .CustomRuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Build();

        [Fact]
        public void Custom_False()
        {
            _comparer
                .Compare(new X { I = 3 }, new X { I = 4 })
                .Should().BeTrue();
        }
        [Fact]
        public void Custom_True()
        {
            _comparer
                .Compare(new X { I = 3 }, new X { I = 40 })
                .Should().BeFalse();
        }

        public class X
        {
            public int I { get; set; }
        }
    }
}