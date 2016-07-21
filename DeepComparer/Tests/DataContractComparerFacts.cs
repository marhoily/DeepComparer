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
        private readonly DataContractComparer _comparer =
            new DataContractComparer();

        [Fact]
        public void Null_And_NonNull_Should_False()
        {
            _comparer.Compare(null, new X { I = 3 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _comparer.Compare(null, null, typeof(X))
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _comparer.Compare(new X { I = 3 }, new X { I = 4 }, typeof(X))
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _comparer.Compare(new X { I = 3 }, new X { I = 3 }, typeof(X))
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            Assert.Throws<TargetException>(()
                => _comparer.Compare(new X(), new Y(), typeof(X)));
        }
        [Fact]
        public void Should_Select_Properties()
        {
            var a = new X { I = 3 };
            var b = new X { I = 3, J = 2 };
            _comparer.Compare(a, b, typeof(X)).Should().BeFalse();
            _comparer.SelectProperties(
                p => p.HasAttribute<DataMemberAttribute>())
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