using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Execution
{
    public static class ExecutionScheduler
    {
        private static readonly Stack<ExecutionInfo> _executionStack = new Stack<ExecutionInfo>();

        static ExecutionScheduler()
            => typeof(StaticUnityMethods).AddHandler("OnUpdate", OnUpdate);

        public static void Schedule<TResult>(this Delegate del, Action<TResult, Exception> callback, object[] args)
            => Schedule(del.GetMethodInfo(), null, callback, args);

        public static void Schedule<TResult>(this MethodBase target, Action<TResult, Exception> callback, object[] args)
            => Schedule(target, null, callback, args);

        public static void Schedule<TResult>(this MethodBase target, object handle, Action<TResult, Exception> callback, object[] args)
            => Schedule(new ExecutionInfo(target, handle, (result, exc) =>
            {
                if (exc != null)
                    callback.SafeCall(default, exc);            
                else if (result is null || result is not TResult tResult)
                    callback.SafeCall(default, exc);
                else
                    callback.SafeCall(tResult, exc);
            }, args));

        public static void Schedule(this Delegate del, Action<object, Exception> callback, object[] args)
            => Schedule(new ExecutionInfo(del.GetMethodInfo(), del.Target, callback, args));

        public static void Schedule(this MethodBase target, object handle, Action<object, Exception> callback, object[] args)
            => Schedule(new ExecutionInfo(target, handle, callback, args));

        public static void Schedule(this ExecutionInfo info)
            => _executionStack.Push(info);

        private static void OnUpdate()
        {
            var startTime = DateTime.Now;

            while (_executionStack.Count > 0 && (DateTime.Now - startTime).TotalMilliseconds <= 200)
            {
                var info = _executionStack.Pop();

                if (info.Target is null)
                    continue;

                try
                {
                    var result = info.Target(info.Handle, info.Args);

                    info.Callback.SafeCall(result, null);
                }
                catch (Exception ex)
                {
                    info.Callback.SafeCall(null, ex);
                }
            }
        }
    }
}