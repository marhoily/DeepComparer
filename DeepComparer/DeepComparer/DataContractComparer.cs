using System;
using System.Linq;
using System.Reflection;

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
            foreach (var p in type.GetProperties().Where(_propSelector))
            {
                var xV = p.GetValue(x, null);
                var yV = p.GetValue(y, null);
                if (xV == null && yV == null)
                    continue;
                if (xV == null || yV == null)
                    return false;
                if (_delveInto(p))
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

        public DataContractComparer SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
            return this;
        }


        private Func<PropertyInfo, bool> _propSelector = x => true;
        private readonly PropertyInfo _cache;
        private Func<PropertyInfo, bool> _delveInto = x => false;


        public DataContractComparer DelveInto(Func<PropertyInfo, bool> func)
        {
            _delveInto = func;
            return this;
        }
    }
}