using Compendium.Logging;

using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class MethodUtilities
    {
        public static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        public static MethodInfo[] GetAllMethods(this Type type)
            => type.GetMethods(BindingFlags);

        public static void SafeCall<TValue>(this Action<TValue> action, TValue value)
        {
            if (action is null)
                return;

            try
            {
                action(value);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call action '{action.Method.ToName()}' due to an exception", ex);
            }
        }

        public static bool SafeCall<TValue>(this Predicate<TValue> predicate, TValue value)
        {
            if (predicate is null)
                return false;

            try
            {
                return predicate(value);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call predicate '{predicate.Method.ToName()}' due to an exception", ex);
                return false;
            }
        }

        public static TResult SafeCall<TResult>(this Func<TResult> func)
        {
            if (func is null)
                return default;

            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call function '{func.Method.ToName()}' due to an exception", ex);
                return default;
            }
        }
    }
}