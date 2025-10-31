using System;
using System.Reflection;

namespace Core
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute(this PropertyInfo property, Type attributeType)
        {
            var hasIsIdentity = Attribute.IsDefined(property, attributeType);
            return hasIsIdentity;
        }
    }
}