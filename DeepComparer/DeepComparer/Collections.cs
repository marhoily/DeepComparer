using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepComparison
{
    public static class Collections
    {
        public static readonly CollectionPredicate Array = 
            t => !t.IsArray ? null : new TreatObjectAs.Collection(
            CollectionComparisonKind.Equal, 
            t.GetElementType(), x => (IEnumerable)x);

        public static readonly CollectionPredicate List = t =>
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(List<>)) return null;
            return new TreatObjectAs.Collection(
                CollectionComparisonKind.Equal,
                t.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };

        public static readonly CollectionPredicate HashSet = t =>
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(HashSet<>)) return null;
            return new TreatObjectAs.Collection(
                CollectionComparisonKind.Equivalent,
                t.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };

        public static readonly CollectionPredicate Enumerable = t =>
        {
            var ifc = t.GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (ifc == null) return null;
            return new TreatObjectAs.Collection(
                CollectionComparisonKind.Equal,
                ifc.GetGenericArguments()[0],
                x => (IEnumerable) x);
        };
    }
}