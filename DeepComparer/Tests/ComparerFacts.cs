using System.Reflection;
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
        public void Null_And_NonNull_Should_False()
        {
            _builder
                .Build()
                .Compare(null, new X { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _builder
                .Build()
                .Compare<X>(null, null)
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _builder
                .Build()
                .Compare(new X { I = 3 }, new X { I = 4 })
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _builder
                .ExpandObjects(t => t == typeof(X))
                .Build()
                .Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            _builder.ExpandObjects(t => t == typeof(X));
            Assert.Throws<TargetException>(()
                => _builder.Build().Compare(new X(), new Y(), typeof(X)));
        }

        public class X { public int I { get; set; } }
        public class Y { public int I { get; set; } }
    }
}