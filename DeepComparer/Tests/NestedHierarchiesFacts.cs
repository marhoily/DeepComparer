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

        [Fact]
        public void Deep_Equal()
        {
            var a = new X { Px = new X { I = 3 } };
            var b = new X { Px = new X { I = 3 } };
            _comparer.Compare(a, b, typeof(X)).Should().BeFalse();
            _comparer.DelveInto(p => p.HasAttribute<DataContractAttribute>())
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }
        [Fact]
        public void Deep_Different()
        {
            var a = new X { Px = new X { I = 3 } };
            var b = new X { Px = new X { I = 4 } };
            _comparer.DelveInto(p => p.HasAttribute<DataContractAttribute>())
                .Compare(a, b, typeof(X)).Should().BeFalse();
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
        public class Y{ }
    }
}