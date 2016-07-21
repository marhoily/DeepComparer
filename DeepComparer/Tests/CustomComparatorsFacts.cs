using System;
using DeepComparer;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class CustomComparatorsFacts
    {
        private readonly DataContractComparer _comparer =
            new DataContractComparer();
        public class X
        {
            public int I { get; set; }
        }
        [Fact]
        public void Custom_False()
        {
            _comparer
                .RuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Compare(new X { I = 3 }, new X { I = 40 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void Custom_True()
        {
            _comparer
                .RuleFor<int>((x, y) => Math.Abs(x - y) < 2)
                .Compare(new X { I = 3 }, new X { I = 40 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void Custom_Twice()
        {
            _comparer.RuleFor<int>((x, y) => false);
            Assert.Throws<ArgumentException>(
                () => _comparer.RuleFor<int>((x, y) => false));
        }
    }
}