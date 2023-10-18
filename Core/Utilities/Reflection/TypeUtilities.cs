using Compendium.Extensions;

using System;
using System.Linq;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class TypeUtilities
    {
        public static MethodInfo Method(this Type type, string name)
            => type.GetMethod(name, MethodUtilities.BindingFlags);

        public static MethodInfo Method(this Type type, string name, params Type[] typeArguments)
            => type.GetMethod(name, MethodUtilities.BindingFlags, null, typeArguments, null);

        public static MethodInfo Method(this Type type, string name, Type genericArg, params Type[] typeArguments)
        {
            var methods = type.GetAllMethods();

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != name)
                    continue;

                var typeArgs = methods[i].GetParameters().Select(p => p.ParameterType);

                if (!typeArgs.IsMatch(typeArguments))
                    continue;

                var genericArgValue = methods[i].GetGenericArguments();

                if (!genericArgValue.Contains(genericArg))
                    continue;

                return methods[i];
            }

            return null;
        }

        public static ConstructorInfo[] GetAllConstructors(this Type type)
            => type.GetConstructors(MethodUtilities.BindingFlags);

        public static Type GetFirstGenericType(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var genericArguments = type.GetGenericArguments();

            if (genericArguments is null || genericArguments.Length <= 0)
                throw new InvalidOperationException($"Attempted to get generic arguments of a type that does not have any.");

            return genericArguments[0];
        }

        public static bool IsStatic(this Type type)
            => type.IsSealed && type.IsAbstract;

        public static bool InheritsType<TType>(this Type type)
            => type.InheritsType(typeof(TType));

        public static bool InheritsType(this Type type, Type inherit)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (inherit is null)
                throw new ArgumentNullException(nameof(inherit));

            if (type.BaseType is null)
                return false;

            if (type.BaseType == inherit)
                return true;

            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType == inherit)
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}