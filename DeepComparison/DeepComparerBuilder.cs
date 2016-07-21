using System;
using System.Reflection;

namespace DeepComparison
{
    /// <summary>Help comparer to choose which objects to treat as collections</summary>
    public delegate TreatObjectAs.Collection CollectionPredicate(Type type);
    /// <summary>Helps building <see cref="DeepComparer"/> using fluent syntax </summary>
    public sealed class DeepComparerBuilder
    {
        private readonly ObjectExpander _objectExpander = new ObjectExpander();
        private readonly RulesContainer _rulesContainer = new RulesContainer();
        /// <summary>Comparer compares all properties by default. Choose which you like</summary>
        public DeepComparerBuilder FilterProperties(Func<PropertyInfo, bool> selector)
        {
            _objectExpander.SelectProperties(selector);
            return this;
        }
        /// <summary>Comparer compares all object as 
        ///     <see cref="object.Equals(object)"/> by default. 
        ///     Choose objects you would like to treat as properties bags</summary>
        public DeepComparerBuilder GoDeepFor(Func<Type, bool> func)
        {
            _rulesContainer.DelveInto(func);
            return this;
        }
        /// <summary>Comparer compares all object as 
        ///     <see cref="object.Equals(object)"/> by default. 
        ///     Choose objects you would like to treat as collections</summary>
        public DeepComparerBuilder GoDeepFor(CollectionPredicate func)
        {
            _rulesContainer.TreatAsCollection(func);
            return this;
        }
        /// <summary>Comparer compares all object as 
        ///     <see cref="object.Equals(object)"/> by default. 
        ///     Choose objects you would like to apply 
        ///     your custom compare function to</summary>
        public DeepComparerBuilder CustomRuleFor<T>(Func<T, T, bool> func)
        {
            _rulesContainer.RuleFor(func);
            return this;
        }

        /// <summary>Returns an instance of <see cref="DeepComparer"/></summary>
        /// <remarks>You can call it and other methods as many times as you like</remarks>
        public DeepComparer Build()
        {
            return new DeepComparer(_objectExpander, _rulesContainer);
        }
    }
}