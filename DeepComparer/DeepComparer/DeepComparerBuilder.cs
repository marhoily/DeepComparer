using System;
using System.Reflection;

namespace DeepComparison
{
    public sealed class DeepComparerBuilder
    {
        private readonly ObjectExpander _objectExpander = new ObjectExpander();
        private readonly RulesContainer _rulesContainer = new RulesContainer();

        public DeepComparerBuilder SelectProperties(Func<PropertyInfo, bool> selector)
        {
            _objectExpander.SelectProperties(selector);
            return this;
        }
        public DeepComparerBuilder ExpandObjects(Func<Type, bool> func)
        {
            _rulesContainer.DelveInto(func);
            return this;
        }
        public DeepComparerBuilder ExpandCollections(Func<Type, TreatObjectAs.Collection> func)
        {
            _rulesContainer.TreatAsCollection(func);
            return this;
        }
        public DeepComparerBuilder CustomRuleFor<T>(Func<T, T, bool> func)
        {
            _rulesContainer.RuleFor(func);
            return this;
        }

        public DeepComparer Build()
        {
            return new DeepComparer(_objectExpander, _rulesContainer);
        }
    }
}