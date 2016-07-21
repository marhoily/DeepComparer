using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparison
{
    internal sealed class RulesContainer
    {
        private readonly List<Func<Type, TreatObjectAs>>
            _byFunc = new List<Func<Type, TreatObjectAs>>();
        public void DelveInto(Func<Type, bool> func)
        {
            _byFunc.Add(t => func(t)
                ? TreatObjectAs.PropertiesBag
                : TreatObjectAs.Simple);
        }
        public void TreatAsCollection(Func<Type, TreatObjectAs.Collection> func)
        {
            _byFunc.Add(x => func(x) ?? TreatObjectAs.Simple);
        }
        public void RuleFor<T>(Func<T, T, bool> func)
        {
            _byFunc.Add(t => t != typeof(T)
                ? TreatObjectAs.Simple
                : new TreatObjectAs.Custom((x, y) =>
                {
                    if (x == null && y == null) return true;
                    if (x == null || y == null) return false;
                    return func((T) x, (T) y);
                }));
        }

        public TreatObjectAs this[Type propertyType]
        {
            get
            {
                return _byFunc
                    .Select(predicate => predicate(propertyType))
                    .SingleOrDefault(x => x != TreatObjectAs.Simple)
                       ?? TreatObjectAs.Simple;
            }
        }

    }
}