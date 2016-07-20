using System;
using System.Linq;

namespace DeepComparer
{
    public sealed class TypeCache
    {
        public PropertyCache[] Properties { get; }
        private readonly Type _type;

        public TypeCache(Type type)
        {
            _type = type;
            Properties = _type.GetProperties()
                .Select(p => new PropertyCache(p))
                .ToArray();
        }
    }
}