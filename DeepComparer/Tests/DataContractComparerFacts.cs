using System.Runtime.Serialization;
using DeepComparer;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public class DataContractComparerFacts
    {
        private DataContractComparer _comparer;

        [Fact]
        public void Null_And_NonNull_Should_False()
        {
            _comparer = new DataContractComparer();
            _comparer.Compare(null, new X { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _comparer = new DataContractComparer();
            _comparer.Compare(null, null)
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _comparer = new DataContractComparer();
            _comparer.Compare(new X { I = 3 }, new X { I = 4 })
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _comparer = new DataContractComparer();
            _comparer.Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            _comparer = new DataContractComparer();
            _comparer.Compare(new X { I = 3 }, new Y { I = 3 })
                .Should().BeFalse();
        }
        [DataContract]
        public class X
        {
            [DataMember]
            public int I { get; set; }
        }
        [DataContract]
        public class Y
        {
            [DataMember]
            public int I { get; set; }
        }
    }
}