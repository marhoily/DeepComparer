using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            _comparer.Compare(a, b, typeof(X)).Should().BeFalse();
            _comparer
                .TreatAsCollection(p => !p.IsArray ? null :
                    new CollectionDescriptor(CollectionComparisonKind.Equal, p.GetElementType(), x => (IEnumerable)x))
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }
        [Fact]
        public void Deep_Unequal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _comparer
                .TreatAsCollection(Defaults.Array)
                .Compare(a, b, typeof(X)).Should().BeFalse();
        }

        [Fact]
        public void Treat_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .TreatAsCollection(Defaults.Array)
                .TreatAsCollection(Defaults.List)
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }
        [Fact]
        public void Treat_Enumerable()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .TreatAsCollection(Defaults.Enumerable)
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }

        public class X
        {
            public X[] A { get; set; }
            public List<X> L { get; set; }
            public HashSet<X> S { get; set; }
        }
    }
}