using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeepComparer;

namespace Tests
{
    public static class Defaults
    {
        public static CompareOption.Collection Array(Type t)
        {
            return !t.IsArray ? null : new CompareOption.Collection(
                CollectionComparisonKind.Equal, 
                t.GetElementType(), x => (IEnumerable)x);
        }
        public static CompareOption.Collection List(Type t)
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(List<>)) return null;
            return new CompareOption.Collection(
                CollectionComparisonKind.Equal, 
                t.GetGenericArguments()[0], 
                x => (IEnumerable)x);
        }
        public static CompareOption.Collection HashSet(Type t)
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(HashSet<>)) return null;
            return new CompareOption.Collection(
                CollectionComparisonKind.Equivalent, 
                t.GetGenericArguments()[0], 
                x => (IEnumerable)x);
        }
        public static CompareOption.Collection Enumerable(Type t)
        {
            var ifc = t.GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (ifc == null) return null;
            return new CompareOption.Collection(
                CollectionComparisonKind.Equal,
                ifc.GetGenericArguments()[0],
                x => (IEnumerable)x);
        }
    }
}