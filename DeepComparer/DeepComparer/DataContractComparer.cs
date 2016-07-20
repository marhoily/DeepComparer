using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DeepComparer.CollectionKind;

namespace DeepComparer
{
    public sealed class DataContractComparer
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;
        private List<Func<Type, bool>> _delveInto =
            new List<Func<Type, bool>>();
        private List<Func<Type, CollectionDescriptor>>
            _treatAsCollection = new List<Func<Type, CollectionDescriptor>>();

        private static readonly Func<object, object, bool> ObjEquals = Equals;

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
        public DataContractComparer TreatAsCollection(
            Func<Type, CollectionDescriptor> func)
        {
            _treatAsCollection.Add(func);
            return this;
        }
        public bool Compare(object x, object y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            var type = x.GetType();
            if (y.GetType() != type)
                return false;
            foreach (var p in type.GetProperties().Where(_propSelector))
            {
                var xV = p.GetValue(x, null);
                var yV = p.GetValue(y, null);
                if (xV == null && yV == null)
                    continue;
                if (xV == null || yV == null)
                    return false;
                var collection = IsCollection(p);
                if (ShouldDelve(p.PropertyType))
                {
                    if (collection != null)
                        throw new Exception("Can't be both!");
                    if (!Compare(xV, yV))
                        return false;
                }
                else if (collection != null)
                {
                    if (!CompareCollection(xV, yV, collection))
                        return false;

                }
                else
                {
                    if (!Equals(xV, yV))
                        return false;
                }
            }
            return true;
        }

        private bool ShouldDelve(Type propertyType)
        {
            return _delveInto.Any(predicate => predicate(propertyType));
        }

        private CollectionDescriptor IsCollection(PropertyInfo p)
        {
            return _treatAsCollection
                .Select(predicate => predicate(p.PropertyType))
                .FirstOrDefault(x => x != null);
        }

        private bool CompareCollection(object xV, object yV, CollectionDescriptor collection)
        {
            var xE = collection.Expand(xV);
            var yE = collection.Expand(yV);
            switch (collection.Kind)
            {
                case Equal:
                    return CollectionEqual(xE, yE, 
                        ShouldDelve(collection.ItemType) ?  Compare : ObjEquals);
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
        public CollectionKind Kind { get; }
        public Type ItemType { get; }
        public Func<object, IEnumerable> Expand { get; }

        public CollectionDescriptor(CollectionKind kind, Type itemType, Func<object, IEnumerable> expand)
        {
            Kind = kind;
            ItemType = itemType;
            Expand = expand;
        }
    }

    public enum CollectionKind
    {
        Equal,
        Equivalent,
        EquivalentSkipDuplicates
    }
}