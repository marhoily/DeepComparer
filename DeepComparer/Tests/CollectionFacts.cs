using System.Collections;
using System.Collections.Generic;
using DeepComparer;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class CollectionFacts
    {
        private readonly DataContractComparer _comparer =
            new DataContractComparer();

        private readonly X _x1 = new X();
        private readonly X _x2 = new X();
        private readonly X _x3 = new X();


        [Fact]
        public void Deep_Equal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _comparer.Compare(a, b).Should().BeFalse();
            _comparer
                .TreatAsCollection(p => !p.PropertyType.IsArray ? null :
                    new CollectionDescriptor(CollectionKind.Equal, p.PropertyType.GetElementType(), x => (IEnumerable)x))
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Deep_Unequal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _comparer
                .TreatAsCollection(p => !p.PropertyType.IsArray
                    ? null
                    : new CollectionDescriptor(
                        CollectionKind.Equal,
                        p.PropertyType.GetElementType(),
                        x => (IEnumerable) x))
                .Compare(a, b).Should().BeFalse();
        }

        public class X
        {
            public X[] A { get; set; }
            public List<X> L { get; set; }
            public HashSet<X> S { get; set; }
        }
    }
}