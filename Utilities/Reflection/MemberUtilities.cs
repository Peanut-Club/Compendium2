using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class MemberUtilities
    {
        private static readonly Dictionary<MemberInfo, string> PreviouslyGeneratedNames = new Dictionary<MemberInfo, string>();

        public static bool HasAttribute<TAttribute>(this MemberInfo member, out TAttribute attribute) where TAttribute : Attribute
            => (attribute = member.GetCustomAttribute<TAttribute>()) != null;

        public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : Attribute
            => member != null && member.IsDefined(typeof(TAttribute));

        public static string ToName(this MemberInfo member)
        {
            if (PreviouslyGeneratedNames.TryGetValue(member, out var name))
                return name;


        }
    }
}