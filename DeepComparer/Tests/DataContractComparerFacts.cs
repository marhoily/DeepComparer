using System.Reflection;
using System.Runtime.Serialization;
using DeepComparer;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace Tests
{
    public class DataContractComparerFacts
    {
        private readonly DataContractComparerBuilder _comparer =
            new DataContractComparerBuilder();

        [Fact]
        public void Null_And_NonNull_Should_False()
        {
            _comparer
                .Build()
                .Compare(null, new X { I = 3 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _comparer
                .Build()
                .Compare(null, null, typeof(X))
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _comparer
                .Build()
                .Compare(new X { I = 3 }, new X { I = 4 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _comparer
                .DelveInto(t => t == typeof(X))
                .Build()
                .Compare(new X { I = 3 }, new X { I = 3 }, typeof(X))
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            _comparer.DelveInto(t => t == typeof(X));
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
                .Compare(a, b, typeof(X)).Should().BeFalse();
            _comparer
                .DelveInto(t => t == typeof(X))
                .SelectProperties(p => p.HasAttribute<DataMemberAttribute>())
                .Build()
                .Compare(a, b, typeof(X)).Should().BeTrue();
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