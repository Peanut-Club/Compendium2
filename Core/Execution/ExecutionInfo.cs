using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Execution
{
    public struct ExecutionInfo
    {
        public readonly FastDelegate Target;

        public readonly object Handle;
        public readonly object[] Args;

        public readonly Action<object, Exception> Callback;

        public ExecutionInfo(FastDelegate target, object handle, object[] args, Action<object, Exception> callback)
        {
            Target = target;
            Handle = handle;
            Args = args;
            Callback = callback;
        }

        public ExecutionInfo(MethodBase target, object handle, Action<object, Exception> callback, object[] args)
        {
            Target = target.GetFastInvoker(true);
            Handle = handle;
            Args = args;
            Callback = callback;
        }

        public ExecutionInfo(MethodBase target, object handle, object[] args) : this(target, handle, null, args) { }
        public ExecutionInfo(MethodBase target, Action<object, Exception> callback, object[] args) : this(target, null, callback, args) { }
        public ExecutionInfo(MethodBase target, object[] args) : this(target, null, null, args) { }
    }
}