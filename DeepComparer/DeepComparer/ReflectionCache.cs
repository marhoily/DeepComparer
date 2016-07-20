using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparer
{
    public sealed class ReflectionCache
    {
        private readonly Dictionary<Type, TypeCache> 
            _typeCache = new Dictionary<Type, TypeCache>();

        public TypeCache Of(Type type)
        {
            TypeCache value;
            if (_typeCache.TryGetValue(type, out value)) return value;
            _typeCache[type] = value = new TypeCache(type);
            value.Fix(this);
            return value;
        }

        public TypeCache TypeOf(object xV)
        {
            return Of(xV.GetType());
        }
    }
}