using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparer
{
    public sealed class TypeCache
    {
        private readonly Dictionary<Type, Attribute> _monoAttrByType;
        public PropertyCache[] Properties { get; }
        private readonly Type _type;

        public TypeCache(Type type)
        {
            _type = type;
            Properties = _type.GetProperties()
                .Select(p => new PropertyCache(p))
                .ToArray();
            _monoAttrByType = _type.GetCustomAttributes(false)
                .GroupBy(arrt => arrt.GetType())
                .Select(g => g.SingleOrDefault())
                .Where(attr => attr != null)
                .ToDictionary(attr => attr.GetType(), attr => (Attribute) attr);
        }

        public bool HasAttribute<T>()
        {
            return _monoAttrByType.ContainsKey(typeof(T));
        }

        public void Fix(ReflectionCache reflectionCache)
        {
            foreach (var propertyCache in Properties)
            {
                propertyCache.Fix(reflectionCache);
            }
        }
    }
}