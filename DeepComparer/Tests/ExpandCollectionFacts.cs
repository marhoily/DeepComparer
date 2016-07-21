using System.Collections;
using System.Collections.Generic;
using DeepComparison;
using FluentAssertions;
using Xunit;
using static DeepComparison.CollectionComparisonKind;

namespace Tests
{
    public sealed class ExpandCollectionFacts
    {
        private readonly DeepComparerBuilder
            _comparer = new DeepComparerBuilder()
                .ExpandObjects(t => t == typeof(X));

        private readonly X _x1 = new X();
        private readonly X _x2 = new X {S = new HashSet<X>()};

        [Fact]
        public void False_Unless_Expand()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _comparer.Build().Compare(a, b).Should().BeFalse();

        }
        [Fact]
        public void True_When_Expanded()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _comparer
                .ExpandCollections(p => !p.IsArray ? null :
                    new TreatObjectAs.Collection(
                        Equal, 
                        p.GetElementType(),
                        x => (IEnumerable)x))
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Deep_Unequal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _comparer
                .ExpandCollections(Defaults.Array)
                .Build()
                .Compare(a, b).Should().BeFalse();
        }

        [Fact]
        public void Expand_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .ExpandCollections(Defaults.Array)
                .ExpandCollections(Defaults.List)
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Expand_Enumerable()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .ExpandCollections(Defaults.Enumerable)
                .Build()
                .Compare(a, b).Should().BeTrue();
        }

        public class X
        {
            public X[] A { get; set; }
            public List<X> L { get; set; }
            public HashSet<X> S { get; set; }
        }
    }
}