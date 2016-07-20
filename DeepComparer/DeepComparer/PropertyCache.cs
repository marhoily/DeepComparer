using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DeepComparer
{
    [DebuggerVisualizer("{_prop.DeclaringType.Name}.{_prop.Name}")]
    public sealed class PropertyCache
    {
        private readonly PropertyInfo _prop;
        private readonly Dictionary<Type, Attribute> _monoAttrByType;

        public PropertyCache(PropertyInfo prop)
        {
            _prop = prop;
            _monoAttrByType = _prop.GetCustomAttributes()
                .GroupBy(arrt => arrt.GetType())
                .Select(g => g.SingleOrDefault())
                .Where(attr => attr != null)
                .ToDictionary(attr => attr.GetType(), attr => attr);
        }

        public TypeCache PropType { get; set; }

        public bool HasAttribute<T>()
        {
            return _monoAttrByType.ContainsKey(typeof(T));
        }

        public object this[object o] => _prop.GetValue(o, null);

        public void Fix(ReflectionCache reflectionCache)
        {
            PropType = reflectionCache.Of(_prop.PropertyType);
        }
    }
}