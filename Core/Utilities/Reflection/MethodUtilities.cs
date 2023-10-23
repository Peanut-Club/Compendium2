using Compendium.Logging;

using HarmonyLib;

using MonoMod.Utils;

using System;
using System.Reflection;

namespace Compendium.Utilities.Reflection
{
    public static class MethodUtilities
    {
        public static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        public static MethodInfo[] GetAllMethods(this Type type)
            => type.GetMethods(BindingFlags);

        public static object SafeCall(this FastDelegate fastDelegate, object target, params object[] args)
        {
            if (fastDelegate is null)
                return null;

            try
            {
                return fastDelegate(target, args);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call fast delegate handler '{fastDelegate.Method.ToName()}' due to an exception", ex);
            }

            return null;
        }

        public static object SafeCall(this FastReflectionHelper.FastInvoker fastInvokeHandler, object target, object[] args)
        {
            if (fastInvokeHandler is null)
                return null;

            try
            {
                return fastInvokeHandler(target, args);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call fast invocation handler '{fastInvokeHandler.Method.ToName()}' due to an exception", ex);
            }

            return null;
        }

        public static object SafeCall(this FastInvokeHandler fastInvokeHandler, object target, object[] args)
        {
            if (fastInvokeHandler is null)
                return null;

            try
            {
                return fastInvokeHandler(target, args);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call fast invocation handler '{fastInvokeHandler.Method.ToName()}' due to an exception", ex);
            }

            return null;
        }

        public static void SafeCall<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2)
        {
            if (action is null)
                return;

            try
            {
                action(t1, t2);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call action '{action.Method.ToName()}' due to an exception", ex);
            }
        }

        public static void SafeCall<T1, T2, T3>(this Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            if (action is null)
                return;

            try
            {
                action(t1, t2, t3);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call action '{action.Method.ToName()}' due to an exception", ex);
            }
        }

        public static void SafeCall(this Action action)
        {
            if (action is null)
                return;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call action '{action.Method.ToName()}' due to an exception", ex);
            }
        }

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

        public static TResult SafeCall<TValue, TResult>(this Func<TValue, TResult> func, TValue value)
        {
            if (func is null)
                return default;

            try
            {
                return func(value);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call function '{func.Method.ToName()}' due to an exception", ex);
                return default;
            }
        }

        public static TResult SafeCall<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 t1, T2 t2)
        {
            if (func is null)
                return default;

            try
            {
                return func(t1, t2);
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to call function '{func.Method.ToName()}' due to an exception", ex);
                return default;
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

        public static bool TryCreateDelegate<TDelegate>(this MethodBase method, object target, out TDelegate del) where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsStatic && !ObjectUtilities.VerifyClassInstanceForMember(method, target))
                throw new ArgumentNullException(nameof(target));

            try
            {
                del = method.CreateDelegate(typeof(TDelegate), target) as TDelegate;

                return del != null;
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to create delegate '{typeof(TDelegate).FullName}' for method '{method.ToName()}'", ex);

                del = null;
                return false;
            }
        }

        public static bool TryCreateDelegate(this MethodBase method, Type delegateType, out Delegate del)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsStatic)
                throw new ArgumentException($"Use the other overload for non-static methods!");

            try
            {
                del = method.CreateDelegate(delegateType);

                return del != null;
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to create delegate '{delegateType.FullName}' for method '{method.ToName()}'", ex);

                del = null;
                return false;
            }
        }

        public static bool TryCreateDelegate(this MethodBase method, object target, Type delegateType, out Delegate del)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsStatic && !ObjectUtilities.VerifyClassInstanceForMember(method, target))
                throw new ArgumentNullException(nameof(target));

            try
            {
                del = method.CreateDelegate(delegateType, target);

                return del != null;
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to create delegate '{delegateType.FullName}' for method '{method.ToName()}'", ex);

                del = null;
                return false;
            }
        }

        public static bool TryCreateDelegate<TDelegate>(this MethodBase method, out TDelegate del) where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsStatic)
                throw new ArgumentException($"Use the other overload for non-static methods!");

            try
            {
                del = method.CreateDelegate(typeof(TDelegate)) as TDelegate;
                return del != null;
            }
            catch (Exception ex)
            {
                Log.Critical($"Failed to create delegate '{typeof(TDelegate).FullName}' for method '{method.ToName()}'", ex);

                del = null;
                return false;
            }
        }
    }
}