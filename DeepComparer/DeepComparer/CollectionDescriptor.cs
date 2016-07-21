using System;
using System.Collections;

namespace DeepComparer
{
    public sealed class CollectionDescriptor
    {
        public CollectionComparisonKind ComparisonKind { get; }
        public Type ItemType { get; }
        public Func<object, IEnumerable> Expand { get; }

        public CollectionDescriptor(CollectionComparisonKind comparisonKind, Type itemType, Func<object, IEnumerable> expand)
        {
            ComparisonKind = comparisonKind;
            ItemType = itemType;
            Expand = expand;
        }
    }
}