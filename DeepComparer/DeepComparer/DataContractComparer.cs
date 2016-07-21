﻿using System;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionComparisonKind;
using static DeepComparer.CompareOption;

namespace DeepComparer
{
    using FCompare = Func<object, object, bool>;
    using LCompare = Func<object, object, Type, bool>;

    public sealed class ObjectExpander
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;

        public void SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
        }
        public bool CompareProperties(object x, object y, Type formalType, LCompare comparer)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return formalType
                .GetProperties()
                .Where(_propSelector)
                .All(p => comparer(
                    p.GetValue(x, null),
                    p.GetValue(y, null),
                    p.PropertyType));
        }
    }
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
            var xE = collection.Expand(x);
            var yE = collection.Expand(y);
            if (collection.ComparisonKind == Equal)
                return xE.SequenceEqual(yE, _cache.Get(collection.ItemType));
            throw new NotImplementedException();
        }
    }
}