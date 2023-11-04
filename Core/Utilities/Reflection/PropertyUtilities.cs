using Fasterflect;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class PropertyUtilities
    {
        private static readonly Dictionary<PropertyInfo, Delegate> _getPropDelegates = new Dictionary<PropertyInfo, Delegate>();
        private static readonly Dictionary<PropertyInfo, Delegate> _setPropDelegates = new Dictionary<PropertyInfo, Delegate>();

        public static PropertyInfo[] GetAllProperties(this Type type)
            => type.Properties(Flags.AllMembers).ToArray();

        public static T GetValueFast<T>(this PropertyInfo prop)
        {
            var value = PropertyInfoExtensions.Get(prop);

            if (value is null || value is not T t)
                return default;

            return t;
        }

        public static T GetValueFast<T>(this PropertyInfo property, object target)
        {
            var value = PropertyInfoExtensions.Get(property, target);

            if (value is null || value is not T t)
                return default;

            return t;
        }

        public static void SetValueFast(this PropertyInfo property, object value)
            => PropertyInfoExtensions.Set(property, value);

        public static void SetValueFast(this PropertyInfo property, object value, object target)
            => PropertyInfoExtensions.Set(property, target, value);
    }
}
