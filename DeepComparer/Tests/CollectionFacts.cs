using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DeepComparison;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public sealed class CollectionFacts
    {
        private readonly DeepComparerBuilder _comparer =
            new DeepComparerBuilder();

        private readonly X _x1 = new X();
        private readonly X _x2 = new X();
        private readonly X _x3 = new X();


        [Fact]
        public void Deep_Equal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _comparer.Build().Compare(a, b, typeof(X)).Should().BeFalse();
            _comparer
                .ExpandObjects(t => t == typeof(X))
                .ExpandCollections(p => !p.IsArray ? null :
                    new TreatObjectAs.Collection(CollectionComparisonKind.Equal, p.GetElementType(), x => (IEnumerable)x))
                .Build()
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }
        [Fact]
        public void Deep_Unequal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _comparer
                .ExpandCollections(Defaults.Array)
                .Build()
                .Compare(a, b, typeof(X)).Should().BeFalse();
        }

        [Fact]
        public void Treat_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .ExpandObjects(t => t == typeof(X))
                .ExpandCollections(Defaults.Array)
                .ExpandCollections(Defaults.List)
                .Build()
                .Compare(a, b, typeof(X)).Should().BeTrue();
        }
        [Fact]
        public void Treat_Enumerable()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .ExpandObjects(t => t == typeof(X))
                .ExpandCollections(Defaults.Enumerable)
                .Build()
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