using System;
using static DeepComparer.CollectionComparisonKind;
using static DeepComparer.CompareOption;

namespace DeepComparer
{
    using FCompare = Func<object, object, bool>;

    public sealed class DataContractComparer
    {
        private readonly ObjectExpander _objectExpander;
        private readonly CompareRules _rules;
        private readonly CachingDictionary<Type, FCompare> _cache;

        public DataContractComparer(
            ObjectExpander objectExpander,
            CompareRules rules)
        {
            _rules = rules;
            _objectExpander = objectExpander;
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
                return (x, y) => _objectExpander.CompareProperties(x, y, formalType, Compare);
            var collection = compareOption as Collection;
            if (collection != null)
                return (x, y) => CompareCollection(x, y, collection);
            var custom = compareOption as Custom;
            return custom != null ? custom.Comparer : Equals;
        }
      
        private bool CompareCollection(object x, object y, Collection collection)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            var xE = collection.ToEnumerable(x);
            var yE = collection.ToEnumerable(y);
            if (collection.ComparisonKind == Equal)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            throw new NotImplementedException();
        }
    }
}