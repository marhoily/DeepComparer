using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionComparisonKind;

namespace DeepComparer
{
    public sealed class DataContractComparer
    {
        private static readonly Func<object, object, bool> ObjEquals = Equals;
        private Func<PropertyInfo, bool> _propSelector = x => true;
        private readonly List<Func<Type, bool>>
            _delveInto = new List<Func<Type, bool>>();
        private readonly List<Func<Type, CollectionDescriptor>>
            _treatAsCollection = new List<Func<Type, CollectionDescriptor>>();
        private readonly Dictionary<Type, Func<object, object, bool>>
            _rules = new Dictionary<Type, Func<object, object, bool>>();

        public DataContractComparer SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
            return this;
        }
        public DataContractComparer DelveInto(Func<Type, bool> func)
        {
            _delveInto.Add(func);
            return this;
        }
        public DataContractComparer TreatAsCollection(Func<Type, CollectionDescriptor> func)
        {
            _treatAsCollection.Add(func);
            return this;
        }
        public DataContractComparer RuleFor<T>(Func<T, T, bool> func)
        {
            _rules.Add(typeof(T), (x, y) => func((T)x, (T)y));
            return this;
        }
        public bool Compare(object x, object y, Type formalType)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            var collection = IsCollection(formalType);
            Func<object, object, bool> customRule;
            _rules.TryGetValue(formalType, out customRule);
            var shouldDelve = ShouldDelve(formalType);
            var countWays = 0;
            if (collection != null) countWays++;
            if (shouldDelve) countWays++;
            if (customRule != null) countWays++;
            if (countWays > 1) throw new Exception("Can't be both!");
            if (shouldDelve)
            {
                return CompareProperties(x, y, formalType);
            }
            if (collection != null)
            {
                return CompareCollection(x, y, collection);
            }
            if (customRule != null)
            {
                return customRule(x, y);
            }
            return Equals(x, y);
        }

        private bool CompareProperties(object x, object y, Type formalType)
        {
            foreach (var p in formalType.GetProperties().Where(_propSelector))
            {
                var xV = p.GetValue(x, null);
                var yV = p.GetValue(y, null);
                if (xV == null && yV == null)
                    continue;
                if (xV == null || yV == null)
                    return false;
                if (!Compare(xV, yV, p.PropertyType)) return false;
            }
            return true;
        }

        private bool ShouldDelve(Type propertyType)
        {
            return _delveInto.Any(predicate => predicate(propertyType));
        }

        private CollectionDescriptor IsCollection(Type propertyType)
        {
            return _treatAsCollection
                .Select(predicate => predicate(propertyType))
                .FirstOrDefault(x => x != null);
        }

        private bool CompareCollection(object xV, object yV, CollectionDescriptor collection)
        {
            var xE = collection.Expand(xV);
            var yE = collection.Expand(yV);
            switch (collection.ComparisonKind)
            {
                case Equal:
                    return CollectionEqual(xE, yE,
                        ShouldDelve(collection.ItemType)
                            ? (a, b) => Compare(a, b, collection.ItemType)
                            : ObjEquals);
                case Equivalent:
                    return CollectionEquivalent(xE, yE, collection.ItemType);
                case EquivalentSkipDuplicates:
                    return CollectionEquivalentSkipDuplicates(xE, yE, collection.ItemType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool CollectionEqual(IEnumerable xE, IEnumerable yE, Func<object, object, bool> compare)
        {
            var xEr = xE.GetEnumerator();
            var yEr = yE.GetEnumerator();
            while (xEr.MoveNext())
            {
                if (!yEr.MoveNext()) return false;
                if (!compare(xEr.Current, yEr.Current)) return false;
            }
            return !yEr.MoveNext();
        }

        private bool CollectionEquivalent(IEnumerable xE, IEnumerable yE, Type itemType)
        {
            throw new NotImplementedException();
        }
        private bool CollectionEquivalentSkipDuplicates(IEnumerable xE, IEnumerable yE, Type itemType)
        {
            throw new NotImplementedException();
        }


    }

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

    public enum CollectionComparisonKind
    {
        Equal,
        Equivalent,
        EquivalentSkipDuplicates
    }
}