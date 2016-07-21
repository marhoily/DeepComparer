using System;
using System.Collections;

namespace DeepComparison
{
    /// <summary>Generic mechanism to match objects with comparers</summary>
    public abstract class TreatObjectAs
    {
        /// <summary>Treat an object as a collection</summary>
        public sealed class Collection : TreatObjectAs
        {
            /// <summary>Choose a way to compare collections</summary>
            public CollectionComparison Comparison { get; }
            /// <summary>Comparer will assume collection items to be of this type</summary>
            public Type ItemType { get; }
            /// <summary>Comparer will use this function to access the object</summary>
            public Func<object, IEnumerable> ToEnumerable { get; }

            /// <summary>initializes all props</summary>
            public Collection(CollectionComparison comparison, Type itemType, Func<object, IEnumerable> expand)
            {
                Comparison = comparison;
                ItemType = itemType;
                ToEnumerable = expand;
            }
        }
        /// <summary>Treat object as requiring a custom comparison</summary>
        public sealed class Custom : TreatObjectAs
        {
            /// <summary>Your comparison function</summary>
            public Func<object, object, bool> Comparer { get; }

            /// <summary>initializes all props</summary>
            public Custom(Func<object, object, bool> comparer)
            {
                Comparer = comparer;
            }
        }

        private sealed class Special : TreatObjectAs { }
        /// <summary>Tells comparer to compare properties 
        ///     instead of calling <see cref="object.Equals(object)"/> on the object itself</summary>
        public static readonly TreatObjectAs PropertiesBag = new Special();
        /// <summary>Tells comparer to call <see cref="object.Equals(object)"/>
        ///     on this object. </summary>
        public static readonly TreatObjectAs Simple = new Special();
    }
}