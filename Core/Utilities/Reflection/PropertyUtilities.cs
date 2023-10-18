using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class PropertyUtilities
    {
        public static PropertyInfo[] GetAllProperties(this Type type)
            => type.GetProperties(MethodUtilities.BindingFlags);
    }
}
