using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nerdhold
{
    public static class LinqExtensions
    {
        [ThreadStatic]
        private static Random _threadRnd;
        private static Random Rnd => _threadRnd ?? (_threadRnd = new Random());

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
                    .Select(t => { try {
                        var genType = t.MakeGenericType(typeof(T));
                        return new { Constructor = genType.GetConstructor(constructorParamTypeArray), GenType = genType};
                    } catch { return null; } })
                    .Where(t => t?.Constructor != null)
            ).ToList();
            var type = types[Rnd.Next(0, types.Count - 1)];
            var result = (IEnumerable<T>)type.Constructor.Invoke(new object[] { collection });
            if (EnsureRandomCollectionCountMatches && result.Count() != collection.Count()) return collection.ToRandomCollectionType();
            return result;
        }

    }
}
