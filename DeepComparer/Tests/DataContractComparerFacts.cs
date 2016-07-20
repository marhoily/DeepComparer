using System.Linq;
using System.Runtime.Serialization;
using DeepComparer;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class NestedHierarchiesFacts
    {
        private readonly DataContractComparer _comparer =
            new DataContractComparer(new ReflectionCache());

        private readonly Y _y1 = new Y();
        private readonly Y _y2 = new Y();

        [Fact]
        public void DelveIn_Selector()
        {
            var a = new X { Px = new X { I = 3 }, Py = _y1 };
            var b = new X { Px = new X { I = 3 }, Py = _y1 };
            _comparer.Compare(a, b).Should().BeFalse();
            _comparer.DelveInto(
                t => t.HasAttribute<DataContractAttribute>())
                .Compare(a, b).Should().BeTrue();
        }

        [DataContract]
        public class X
        {
            [DataMember]
            public X Px { get; set; }
            public Y Py { get; set; }
            [DataMember]
            public int I { get; set; }
        }
        public class Y
        {
            [DataMember]
            public int I { get; set; }
        }
    }

    public class DataContractComparerFacts
    {
        private readonly DataContractComparer _comparer =
            new DataContractComparer(new ReflectionCache());

        [Fact]
        public void Null_And_NonNull_Should_False()
        {
            _comparer.Compare(null, new X { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void Null_And_Null_Should_False()
        {
            _comparer.Compare(null, null)
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Prop_Should_False()
        {
            _comparer.Compare(new X { I = 3 }, new X { I = 4 })
                .Should().BeFalse();
        }
        [Fact]
        public void When_Same_Prop_Should_True()
        {
            _comparer.Compare(new X { I = 3 }, new X { I = 3 })
                .Should().BeTrue();
        }
        [Fact]
        public void When_Different_Types_Should_False()
        {
            _comparer.Compare(new X { I = 3 }, new Y { I = 3 })
                .Should().BeFalse();
        }
        [Fact]
        public void Should_Select_Properties()
        {
            var a = new X { I = 3 };
            var b = new X { I = 3, J = 2 };
            _comparer.Compare(a, b).Should().BeFalse();
            _comparer.SelectProperties(
                p => p.HasAttribute<DataMemberAttribute>())
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