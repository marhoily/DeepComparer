using System;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionComparisonKind;
using static DeepComparer.CompareOption;

namespace DeepComparer
{
    using FCompare = Func<object, object, bool>;

    public sealed class DataContractComparer
    {
        private readonly Func<PropertyInfo, bool> _propSelector;
        private readonly CompareRules _rules;
        private readonly CachingDictionary<Type, FCompare> _cache;

        public DataContractComparer(
            Func<PropertyInfo, bool> propSelector,
            CompareRules rules)
        {
            _propSelector = propSelector;
            _rules = rules;
            _cache = new CachingDictionary<Type, FCompare>(GetComparer);
        }

        public bool Compare(object x, object y, Type formalType)
        {
            return _cache.Get(formalType)(x, y);
        }
        private FCompare GetComparer(Type formalType)
        {
            var compareOption = _rules[formalType];
            if (compareOption == Expand)
                return (x, y) => CompareProperties(x, y, formalType);
            var collection = compareOption as Collection;
            if (collection != null)
                return (x, y) => CompareCollection(x, y, collection);
            var custom = compareOption as Custom;
            return custom != null ? custom.Comparer : Equals;
        }
        private bool CompareProperties(object x, object y, Type formalType)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return formalType
                .GetProperties()
                .Where(_propSelector)
                .All(p => Compare(
                    p.GetValue(x, null),
                    p.GetValue(y, null),
                    p.PropertyType));
        }
        private bool CompareCollection(object x, object y, Collection collection)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            var xE = collection.Expand(x);
            var yE = collection.Expand(y);
            if (collection.ComparisonKind == Equal)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            throw new NotImplementedException();
        }
    }
}