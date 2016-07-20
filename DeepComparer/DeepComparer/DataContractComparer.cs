using System;
using System.Collections;
using System.Collections.Generic;

namespace DeepComparer
{
    using SeqProp = IEnumerable<PropertyCache>;
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
            foreach (var p in _propSelector(_cache.Of(type).Properties))
            {
                var xV = p[x];
                var yV = p[y];
                if (!Equals(xV, yV)) return false;
            }
            return true;
        }

        public DataContractComparer SelectProperties(Func<SeqProp, SeqProp> selector)
        {
            _propSelector = selector;
            return this;
        }


        private Func<SeqProp, SeqProp> _propSelector = x => x;
        private readonly ReflectionCache _cache;

        public DataContractComparer(ReflectionCache cache)
        {
            _cache = cache;
        }
    }
}