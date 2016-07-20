using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DeepComparer;

namespace Tests
{
    public static class Defaults
    {
        public static CollectionDescriptor Array(Type t)
        {
            return !t.IsArray ? null : new CollectionDescriptor(
                CollectionComparisonKind.Equal, 
                t.GetElementType(), x => (IEnumerable)x);
        }
        public static CollectionDescriptor List(Type t)
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(List<>)) return null;
            return new CollectionDescriptor(
                CollectionComparisonKind.Equal, 
                t.GetGenericArguments()[0], 
                x => (IEnumerable)x);
        }
        public static CollectionDescriptor HashSet(Type t)
        {
            if (!t.IsGenericType) return null;
            if (t.GetGenericTypeDefinition() != typeof(HashSet<>)) return null;
            return new CollectionDescriptor(
                CollectionComparisonKind.Equivalent, 
                t.GetGenericArguments()[0], 
                x => (IEnumerable)x);
        }
        public static CollectionDescriptor Enumerable(Type t)
        {
            var ifc = t.GetInterfaces().SingleOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (ifc == null) return null;
            return new CollectionDescriptor(
                CollectionComparisonKind.Equal,
                ifc.GetGenericArguments()[0],
                x => (IEnumerable)x);
        }
    }
}