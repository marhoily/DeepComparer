using System;

namespace DeepComparison
{
    using FCompare = Func<object, object, bool>;

    public sealed class DeepComparer
    {
        private readonly ObjectExpander _objectExpander;
        private readonly RulesContainer _rulesContainer;
        private readonly CachingDictionary<Type, FCompare> _cache;

        internal DeepComparer(
            ObjectExpander objectExpander,
            RulesContainer rulesContainer)
        {
            _rulesContainer = rulesContainer;
            _objectExpander = objectExpander;
            _cache = new CachingDictionary<Type, FCompare>(GetComparer);
        }

        public bool Compare<T>(T x, T y)
        {
            return _cache.Get(typeof(T))(x, y);
        }
        public bool Compare(object x, object y, Type formalType)
        {
            return _cache.Get(formalType)(x, y);
        }
        private FCompare GetComparer(Type formalType)
        {
            var compareOption = _rulesContainer[formalType];
            if (compareOption == TreatObjectAs.PropertiesBag)
                return (x, y) => _objectExpander.CompareProperties(x, y, formalType, Compare);
            var collection = compareOption as TreatObjectAs.Collection;
            if (collection != null)
                return (x, y) => CompareCollection(x, y, collection);
            var custom = compareOption as TreatObjectAs.Custom;
            return custom != null ? custom.Comparer : Equals;
        }
      
        private bool CompareCollection(object x, object y, TreatObjectAs.Collection collection)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            var xE = collection.ToEnumerable(x);
            var yE = collection.ToEnumerable(y);
            if (collection.ComparisonKind == CollectionComparisonKind.Equal)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            throw new NotImplementedException();
        }
    }
}