using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DeepComparer
{
    [Serializable]
    public sealed class CachingDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _factory;

        public CachingDictionary(Func<TKey, TValue> factory) { _factory = factory; }

        private CachingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TValue Get(TKey key)
        {
            TValue value;
            if (!TryGetValue(key, out value))
                this[key] = value = _factory(key);
            return value;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}