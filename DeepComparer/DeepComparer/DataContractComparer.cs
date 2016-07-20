using System;
using System.Linq;

namespace DeepComparer
{
    public sealed class DataContractComparer
    {
        public bool Compare(object x, object y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            var type = x.GetType();
            if (y.GetType() != type)
                return false;
            foreach (var p in _cache.Of(type).Properties.Where(_propSelector))
            {
                var xV = p[x];
                var yV = p[y];
                if (xV == null && yV == null)
                    continue;
                if (xV == null || yV == null)
                    return false;
                if (_delveInto(_cache.TypeOf(xV)))
                {
                    if (!Compare(xV, yV))
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

        public DataContractComparer SelectProperties(Func<PropertyCache, bool> selector)
        {
            _propSelector = selector;
            return this;
        }


        private Func<PropertyCache, bool> _propSelector = x => true;
        private readonly ReflectionCache _cache;
        private Func<TypeCache, bool> _delveInto = x => false;

        public DataContractComparer(ReflectionCache cache)
        {
            _cache = cache;
        }

        public DataContractComparer DelveInto(Func<TypeCache, bool> func)
        {
            _delveInto = func;
            return this;
        }
    }
}