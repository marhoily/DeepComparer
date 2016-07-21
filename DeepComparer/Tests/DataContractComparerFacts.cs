using System.Reflection;
using System.Runtime.Serialization;
using DeepComparison;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace Tests
{
    public class DataContractComparerFacts
    {
        private readonly DeepComparerBuilder _comparer =
            new DeepComparerBuilder();

        [Fact]
        public void Null_And_NonNull_Should_False()
        {
            _comparer
                .Build()
                .Compare(null, new X { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _comparer
                .Build()
                .Compare<X>(null, null)
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _comparer
                .Build()
                .Compare(new X { I = 3 }, new X { I = 4 })
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _comparer
                .ExpandObjects(t => t == typeof(X))
                .Build()
                .Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            _comparer.ExpandObjects(t => t == typeof(X));
            Assert.Throws<TargetException>(()
                => _comparer.Build().Compare(new X(), new Y(), typeof(X)));
        }
        [Fact]
        public void Should_Select_Properties()
        {
            var a = new X { I = 3 };
            var b = new X { I = 3, J = 2 };
            _comparer
                .Build()
                .Compare(a, b).Should().BeFalse();
            _comparer
                .ExpandObjects(t => t == typeof(X))
                .SelectProperties(p => p.HasAttribute<DataMemberAttribute>())
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        public class X
        {
            [DataMember]
            public int I { get; set; }
            public int J { get; set; }
        }
        public class Y
        {
            [DataMember]
            public int I { get; set; }
        }
    }
}