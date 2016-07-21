using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionComparisonKind;

namespace DeepComparer
{
    using FCompare = Func<object, object, bool>;
    public sealed class DataContractComparer
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;
        private readonly List<Func<Type, bool>>
            _delveInto = new List<Func<Type, bool>>();
        private readonly List<Func<Type, CollectionDescriptor>>
            _treatAsCollection = new List<Func<Type, CollectionDescriptor>>();
        private readonly Dictionary<Type, FCompare>
            _rules = new Dictionary<Type, FCompare>();
        private readonly CachingDictionary<Type, FCompare> _comparers;

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

        public DataContractComparer()
        {
            _comparers = new CachingDictionary<Type, FCompare>(GetComparer);
        }

        private FCompare GetComparer(Type formalType)
        {
            var collection = IsCollection(formalType);
            FCompare customRule;
            _rules.TryGetValue(formalType, out customRule);
            var shouldDelve = ShouldDelve(formalType);
            var countWays = 0;
            if (collection != null) countWays++;
            if (shouldDelve) countWays++;
            if (customRule != null) countWays++;
            if (countWays > 1) throw new Exception("Can't be both!");
            if (shouldDelve)
            {
                return (x, y) => CompareProperties(x, y, formalType);
            }
            if (collection != null)
            {
                return (x, y) => CompareCollection(x, y, collection);
            }
            if (customRule != null)
            {
                return customRule;
            }
            return Equals;
        }
        public bool Compare(object x, object y, Type formalType)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            
            return GetComparer(formalType)(x, y);
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
                    return CollectionEqual(xE, yE, GetComparer(collection.ItemType));
                case Equivalent:
                    return CollectionEquivalent(xE, yE, collection.ItemType);
                case EquivalentSkipDuplicates:
                    return CollectionEquivalentSkipDuplicates(xE, yE, collection.ItemType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool CollectionEqual(IEnumerable xE, IEnumerable yE, FCompare compare)
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
}