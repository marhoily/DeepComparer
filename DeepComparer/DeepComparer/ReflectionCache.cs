using System;

namespace DeepComparer
{
    public sealed class ReflectionCache
    {
        private readonly CachingDictionary<Type, TypeCache>
            _typeCache = new CachingDictionary<Type, TypeCache>(
                t => new TypeCache(t));

        public TypeCache Of(Type type) => _typeCache.Get(type);
    }
}