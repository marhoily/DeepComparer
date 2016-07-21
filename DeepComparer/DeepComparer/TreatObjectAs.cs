using System;
using System.Collections;

namespace DeepComparer
{
    public abstract class TreatObjectAs
    {
        public sealed class Collection : TreatObjectAs
        {
            public CollectionComparisonKind ComparisonKind { get; }
            public Type ItemType { get; }
            public Func<object, IEnumerable> ToEnumerable { get; }

            public Collection(CollectionComparisonKind comparisonKind, Type itemType, Func<object, IEnumerable> expand)
            {
                ComparisonKind = comparisonKind;
                ItemType = itemType;
                ToEnumerable = expand;
            }
        }
        public sealed class Custom : TreatObjectAs
        {
            public Func<object, object, bool> Comparer { get; }

            public Custom(Func<object, object, bool> comparer)
            {
                Comparer = comparer;
            }
        }

        private sealed class Special : TreatObjectAs { }
        public static readonly TreatObjectAs PropertiesBag = new Special();
        public static readonly TreatObjectAs Simple = new Special();
    }
}