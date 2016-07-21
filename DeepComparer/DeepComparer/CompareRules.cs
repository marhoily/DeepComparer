using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparer
{
    public sealed class CompareRules
    {
        private readonly List<Func<Type, CompareOption>>
            _byFunc = new List<Func<Type, CompareOption>>();
        public void DelveInto(Func<Type, bool> func)
        {
            _byFunc.Add(t => func(t)
                ? CompareOption.Expand
                : CompareOption.Skip);
        }
        public void TreatAsCollection(Func<Type, CompareOption.Collection> func)
        {
            _byFunc.Add(x => func(x) ?? CompareOption.Skip);
        }
        public void RuleFor<T>(Func<T, T, bool> func)
        {
            _byFunc.Add(t => t != typeof(T)
                ? CompareOption.Skip
                : new CompareOption.Custom((x, y) =>
                {
                    if (x == null && y == null) return true;
                    if (x == null || y == null) return false;
                    return func((T) x, (T) y);
                }));
        }

        public CompareOption this[Type propertyType]
        {
            get
            {
                return _byFunc
                    .Select(predicate => predicate(propertyType))
                    .SingleOrDefault(x => x != CompareOption.Skip)
                       ?? CompareOption.Skip;
            }
        }

    }
}