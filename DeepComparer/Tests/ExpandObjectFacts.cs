using System.Runtime.Serialization;
using DeepComparison;
using FluentAssertions;
using FluentAssertions.Common;
using Xunit;

namespace Tests
{
    public sealed class ExpandObjectFacts
    {
        private readonly DeepComparer _comparer =
            new DeepComparerBuilder()
                .GoDeepFor(t => t.HasAttribute<DataContractAttribute>())
                .SelectProperties(p => p.HasAttribute<DataMemberAttribute>())
                .Build();

        [Fact]
        public void Should_Select_Properties()
        {
            var a = new X { I = 2 };
            var b = new X { I = 3 };
            _comparer.Compare(a, b).Should().BeFalse();
        }
        [Fact]
        public void Should_Not_Care_For_Others()
        {
            var a = new X { J = 3 };
            var b = new X { J = 2 };
            _comparer.Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Deep_Equal()
        {
            var a = new X { Px = new X { I = 3 } };
            var b = new X { Px = new X { I = 3 } };
            _comparer.Compare(a, b).Should().BeTrue();
        }
        [Fact]
        public void Deep_Different()
        {
            var a = new X { Px = new X { I = 3 } };
            var b = new X { Px = new X { I = 4 } };
            _comparer.Compare(a, b).Should().BeFalse();
        }

        [DataContract]
        public class X
        {
            [DataMember]
            public X Px { get; set; }
            [DataMember]
            public int I { get; set; }
            public int J { get; set; }
        }
    }
}