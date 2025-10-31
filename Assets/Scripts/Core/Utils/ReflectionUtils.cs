using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t != baseType && 
                                                  baseType.IsAssignableFrom(t));
        }
    }
}