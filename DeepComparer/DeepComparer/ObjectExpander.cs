using System;
using System.Linq;
using System.Reflection;

namespace DeepComparison
{
    public sealed class ObjectExpander
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;

        public void SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
        }
        public bool CompareProperties(object x, object y, 
            Type formalType, Func<object, object, Type, bool> comparer)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return formalType
                .GetProperties()
                .Where(_propSelector)
                .All(p => comparer(
                    p.GetValue(x, null),
                    p.GetValue(y, null),
                    p.PropertyType));
        }
    }
}