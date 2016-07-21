using System;
using System.Reflection;

namespace DeepComparer
{
    public sealed class DataContractComparerBuilder
    {
        private readonly ObjectExpander _objectExpander = new ObjectExpander();
        private readonly CompareRules _rules = new CompareRules();

        public DataContractComparerBuilder SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _objectExpander.SelectProperties(selector);
            return this;
        }
        public DataContractComparerBuilder DelveInto(Func<Type, bool> func)
        {
            _rules.DelveInto(func);
            return this;
        }
        public DataContractComparerBuilder TreatAsCollection(Func<Type, TreatObjectAs.Collection> func)
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
            return new DataContractComparer(_objectExpander, _rules);
        }
    }
}