using System.Runtime.Serialization;
using DeepComparer;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace Tests
{
    public sealed class NestedHierarchiesFacts
    {
        private readonly DataContractComparer _comparer =
            new DataContractComparer();

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
}