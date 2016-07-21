using System;
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
            _builder = new DeepComparerBuilder()
                .GoDeepFor(t => t == typeof(X));

        private readonly X _x1 = new X();
        private readonly X _x2 = new X { S = new HashSet<X>() };

        [Fact]
        public void DoNot_GoDeepFor_Collections_ByDefault()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _builder.Build().Compare(a, b).Should().BeFalse();
        }

        [Fact]
        public void GoDeepFor_Collections_Example()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _builder.GoDeepFor(Collections.Array)
                .Build()
                .Compare(a, b).Should().BeFalse();
        }
        [Fact]
        public void Full_Syntax()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x1 } };
            _builder
                .GoDeepFor(t => !t.IsArray ? null :
                    new TreatObjectAs.Collection(
                        Equal,
                        t.GetElementType(),
                        x => (IEnumerable)x))
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Expand_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _builder
                .GoDeepFor(Collections.Array)
                .GoDeepFor(Collections.List)
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Expand_Enumerable()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _builder.GoDeepFor(Collections.Enumerable)
                .Build()
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Expand_Ambiguity()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            Assert.Throws<InvalidOperationException>(() => _builder
                .GoDeepFor(Collections.List)
                .GoDeepFor(Collections.Enumerable)
                .Build()
                .Compare(a, b).Should().BeTrue());
        }

        [Fact]
        public void Collection_Different_Elements()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _builder.GoDeepFor(Collections.Array)
                .Build()
                .Compare(a, b).Should().BeFalse();
        }
        [Fact]
        public void Collection_Different_Sizes()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new X[0]  };
            _builder.GoDeepFor(Collections.Array)
                .Build()
                .Compare(a, b).Should().BeFalse();
        }
        [Fact]
        public void Collection_Null_Element()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new X[] { null } };
            _builder.GoDeepFor(Collections.Array)
                .Build()
                .Compare(a, b).Should().BeFalse();
        }

        [Fact]
        public void Collection_Null_Elements_Equal()
        {
            var a = new X { A = new X[] { null } };
            var b = new X { A = new X[] { null } };
            _builder.GoDeepFor(Collections.Array)
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