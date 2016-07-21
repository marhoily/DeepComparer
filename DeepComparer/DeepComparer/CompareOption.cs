using System;
using System.Collections;

namespace DeepComparer
{
    public abstract class CompareOption
    {
        public sealed class Collection : CompareOption
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
        public sealed class ExpandOpt : CompareOption { }
        public static readonly CompareOption Expand = new ExpandOpt();
        public sealed class SkipOpt : CompareOption { }
        public static readonly CompareOption Skip = new SkipOpt();
        public sealed class Custom : CompareOption
        {
            public Func<object, object, bool> Comparer { get; }

            public Custom(Func<object, object, bool> comparer)
            {
                Comparer = comparer;
            }
        }
    }
}