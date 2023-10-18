using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class ObjectUtilities
    {
        public static bool IsInstance(object instanceOne, object instanceTwo)
        {
            if (instanceOne is null && instanceTwo is null)
                return true;

            if ((instanceOne != null && instanceTwo is null) || (instanceOne is null || instanceTwo != null))
                return false;

            if (instanceOne.GetType() != instanceTwo.GetType())
                return false;

            try
            {
                return instanceOne.Equals(instanceTwo);
            }
            catch
            {
                return ReferenceEquals(instanceOne, instanceTwo);
            }
        }

        public static bool VerifyClassInstanceForMember(MemberInfo member, object instance)
        {
            if (member is MethodBase method)
                return method.IsStatic || instance != null && instance.GetType() == member.DeclaringType;
            else if (member is FieldInfo field)
                return field.IsStatic || instance != null && instance.GetType() == member.DeclaringType;
            else if (member is PropertyInfo property)
                return (property.SetMethod?.IsStatic ?? property.GetMethod?.IsStatic).GetValueOrDefault() || instance != null && instance.GetType() == member.DeclaringType;
            else
                throw new ArgumentException($"Member '{member.MemberType}' is not supported by this method.");
        }
    }
}