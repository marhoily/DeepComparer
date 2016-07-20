using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DeepComparer;
using FluentAssertions;
using Xunit;

namespace Tests
{
    public static class Defaults
    {
        public static CollectionDescriptor Array(Type t)
        {
            return !t.IsArray ? null : new CollectionDescriptor(
                CollectionKind.Equal, t.GetElementType(), x => (IEnumerable)x);
        }
        public static CollectionDescriptor List(Type t)
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(List<>)) return null;
            var elementType = t.GetElementType();
            return new CollectionDescriptor(
                CollectionKind.Equal, elementType, x => (IEnumerable)x);
        }
    }
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
                .TreatAsCollection(p => !p.IsArray ? null :
                    new CollectionDescriptor(CollectionKind.Equal, p.GetElementType(), x => (IEnumerable)x))
                .Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Deep_Unequal()
        {
            var a = new X { A = new[] { _x1 } };
            var b = new X { A = new[] { _x2 } };
            _comparer
                .TreatAsCollection(Defaults.Array)
                .Compare(a, b).Should().BeFalse();
        }

        [Fact]
        public void Treat_Two()
        {
            var a = new X { A = new[] { _x1 }, L = new List<X> {_x2} };
            var b = new X { A = new[] { _x1 }, L = new List<X> { _x2 } };
            _comparer
                .TreatAsCollection(Defaults.Array)
                .TreatAsCollection(Defaults.List)
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