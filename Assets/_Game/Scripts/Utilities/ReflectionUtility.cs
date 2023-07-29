using System;
using System.Collections.Generic;
using System.Linq;

namespace DLS.Game.Utilities
{
    public static class ReflectionUtility
    {
        public static Type[] GetDerivedTypes(Type baseType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var derivedTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
                derivedTypes.AddRange(types);
            }

            return derivedTypes.ToArray();
        }
    }
}