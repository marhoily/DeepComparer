using System;
using System.Reflection;

namespace DeepComparer
{
    public sealed class DataContractComparerBuilder
    {
        private Func<PropertyInfo, bool> _propSelector = x => true;
        private readonly CompareRules _rules = new CompareRules();

        public DataContractComparerBuilder SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _propSelector = selector;
            return this;
        }
        public DataContractComparerBuilder DelveInto(Func<Type, bool> func)
        {
            _rules.DelveInto(func);
            return this;
        }
        public DataContractComparerBuilder TreatAsCollection(Func<Type, CompareOption.Collection> func)
        {
            _rules.TreatAsCollection(func);
            return this;
        }
        public DataContractComparerBuilder RuleFor<T>(Func<T, T, bool> func)
        {
            _rules.RuleFor(func);
            return this;
        }

        public DataContractComparer Build()
        {
            return new DataContractComparer(
                new ObjectExpander(_propSelector), _rules);
        }
    }
}