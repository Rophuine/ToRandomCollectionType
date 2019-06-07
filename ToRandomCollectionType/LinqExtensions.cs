using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nerdhold
{
    public static class LinqExtensions
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Ensures the collection count matches, but will cause multiple enumerations. Choose your risk wisely!
        /// </summary>
        public static bool EnsureRandomCollectionCountMatches = false;

        /// <summary>
        /// Just don't do this in serious code. That should be obvious, right?
        /// Seriously, there are several ways this could break on any given run. No attempt has been made to make it safe.
        /// </summary>
        public static IEnumerable<T> ToRandomCollectionType<T>(this IEnumerable<T> collection)
        {
            var constructorParamTypeArray = new[] { collection.GetType() };
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
                assembly.GetTypes()
                    .Where(t =>
                        typeof(IEnumerable).IsAssignableFrom(t)
                        && t.GetGenericArguments().Length == 1
                        && t.IsGenericTypeDefinition
                    )
                    .Select(t => { try
                    {
                        t.GetConstructor(constructorParamTypeArray); return t.MakeGenericType(typeof(T)); } catch { return null; } })
                    .Where(t => t != null && t.GetConstructor(constructorParamTypeArray) != null)
            ).ToList();
            var type = types[Rnd.Next(0, types.Count - 1)];
            var constructor = type.GetConstructor(constructorParamTypeArray);
            if (constructor == null) throw new Exception("Panic: Couldn't find a useful constructor.");
            var result = (IEnumerable<T>)constructor.Invoke(new object[] { collection });
            if (EnsureRandomCollectionCountMatches && result.Count() != collection.Count()) return collection.ToRandomCollectionType();
            return result;
        }

    }
}
