using System.Reflection;
using System;

using Compendium.Utilities.Reflection;

namespace Compendium.Configuration
{
    public static class ConfigurationUtils
    {
        public static bool IsBusy(this ConfigurationStatus status)
            => status is ConfigurationStatus.Loading || status is ConfigurationStatus.Saving;

        public static object GetValue(object handle, MemberInfo target) 
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!ObjectUtilities.VerifyClassInstanceForMember(target, handle))
                throw new ArgumentNullException(nameof(handle));

            if (target is FieldInfo field)
                return field.GetValue(handle);
            else if (target is PropertyInfo property)
                return property.GetValue(handle);
            else
                throw new ArgumentException($"Member type '{target.MemberType}' is not valid.");
        }

        public static void SetValue(object handle, object value, MemberInfo target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!ObjectUtilities.VerifyClassInstanceForMember(target, handle))
                throw new ArgumentNullException(nameof(handle));

            if (target is FieldInfo field)
                field.SetValue(handle, value);
            else if (target is PropertyInfo property)
                property.SetValue(handle, value);
            else
                throw new ArgumentException($"Member type '{target.MemberType}' is not valid.");
        }
    }
}