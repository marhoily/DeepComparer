﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionComparisonKind;

namespace DeepComparer
{
    using FCompare = Func<object, object, bool>;

    public abstract class CompareOption
    {
        public sealed class Collection : CompareOption
        {
            public CollectionComparisonKind ComparisonKind { get; }
            public Type ItemType { get; }
            public Func<object, IEnumerable> Expand { get; }

            public Collection(CollectionComparisonKind comparisonKind, Type itemType, Func<object, IEnumerable> expand)
            {
                ComparisonKind = comparisonKind;
                ItemType = itemType;
                Expand = expand;
            }
        }
        public sealed class ExpandOpt : CompareOption { }
        public static readonly CompareOption Expand = new ExpandOpt();
        public sealed class SkipOpt : CompareOption { }
        public static readonly CompareOption Skip = new SkipOpt();
        public sealed class Custom : CompareOption
        {
            public FCompare Comparer { get; }

            public Custom(FCompare comparer)
            {
                Comparer = comparer;
            }
        }
    }

    public sealed class CompareRules
    {
        private readonly List<Func<Type, CompareOption>>
            _byFunc = new List<Func<Type, CompareOption>>();
        public void DelveInto(Func<Type, bool> func)
        {
            _byFunc.Add(t => func(t)
                ? CompareOption.Expand
                : CompareOption.Skip);
        }
        public void TreatAsCollection(Func<Type, CompareOption.Collection> func)
        {
            _byFunc.Add(func);
        }
        public void RuleFor<T>(Func<T, T, bool> func)
        {
            _byFunc.Add(t => t != typeof(T)
                ? CompareOption.Skip
                : new CompareOption.Custom((x, y) =>
                {
                    if (x == null && y == null)
                        return true;
                    if (x == null || y == null)
                        return false;
                    return func((T) x, (T) y);
                }));
        }
        private CompareOption this[Type propertyType]
        {
            get
            {
                return _byFunc
                    .Select(predicate => predicate(propertyType))
                    .SingleOrDefault(x => x != CompareOption.Skip);
            }
        }

    }
    public sealed class DataContractComparerBuilder
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;
        private readonly List<Func<Type, bool>>
            _delveInto = new List<Func<Type, bool>>();
        private readonly List<Func<Type, CollectionDescriptor>>
            _treatAsCollection = new List<Func<Type, CollectionDescriptor>>();
        private readonly Dictionary<Type, FCompare>
            _rules = new Dictionary<Type, FCompare>();
        public DataContractComparerBuilder SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
            return this;
        }
        public DataContractComparerBuilder DelveInto(Func<Type, bool> func)
        {
            _delveInto.Add(func);
            return this;
        }
        public DataContractComparerBuilder TreatAsCollection(Func<Type, CollectionDescriptor> func)
        {
            _treatAsCollection.Add(func);
            return this;
        }
        public DataContractComparerBuilder RuleFor<T>(Func<T, T, bool> func)
        {
            _rules.Add(typeof(T), (x, y) =>
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false; return func((T)x, (T)y);
            });
            return this;
        }

        public DataContractComparer Build()
        {
            return new DataContractComparer(
                _propSelector, _delveInto, _treatAsCollection, _rules);
        }
    }
    public sealed class DataContractComparer
    {
        private readonly Func<PropertyInfo, bool> _propSelector;
        private readonly List<Func<Type, bool>>            _delveInto;
        private readonly List<Func<Type, CollectionDescriptor>> _treatAsCollection;
        private readonly Dictionary<Type, FCompare> _rules;
        private readonly CachingDictionary<Type, FCompare> _comparers;

        public DataContractComparer(
            Func<PropertyInfo, bool> propSelector, 
            List<Func<Type, bool>> delveInto, 
            List<Func<Type, CollectionDescriptor>> treatAsCollection,
            Dictionary<Type, FCompare> rules)
        {
            _propSelector = propSelector;
            _delveInto = delveInto;
            _treatAsCollection = treatAsCollection;
            _rules = rules;
            _comparers = new CachingDictionary<Type, FCompare>(GetComparer);
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
            return _comparers.Get(formalType)(x, y);
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


        private bool CompareCollection(object x, object y, CollectionDescriptor collection)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            var xE = collection.Expand(x);
            var yE = collection.Expand(y);
            if (collection.ComparisonKind == Equal)
                return CollectionEqual(xE, yE, _comparers.Get(collection.ItemType));
            throw new NotImplementedException();
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
    }
}