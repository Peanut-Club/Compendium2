using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Reflection;
using System.Collections.Generic;
using Compendium.Profiling;

namespace Compendium.Utilities.Calls
{
    public class CallInfo
    {
        private static readonly List<CallInfo> _allCallers = new List<CallInfo>();

        private CallFlags _flagsValue = CallFlags.None;

        public Delay Delay { get; set; }

        public CallFlags Flags
        {
            get => _flagsValue;
            set
            {
                if (_flagsValue != value)
                {
                    _flagsValue = value;

                    IsDelayEnabled = _flagsValue.Any(CallFlags.EnableDelay);
                    IsProfilerEnabled = _flagsValue.Any(CallFlags.EnableProfiler);
                }
            }
        }

        public MethodInfo Method { get; }
        public FastReflectionHelper.FastInvoker Invoker { get; }

        public ProfilerRecord Profiler { get; }

        public object[] OverloadBuffer { get; }
        public object Handle { get; }

        public bool IsDelayEnabled { get; private set; }
        public bool IsProfilerEnabled { get; private set; }

        public CallInfo(
            MethodInfo method, 
            FastReflectionHelper.FastInvoker invoker, 
            
            object[] overloadBuffer, 
            object handle, 
            
            CallFlags flags, 
            
            Delay delay)
        {
            Flags = flags;
            Delay = delay;
            Method = method;
            Invoker = invoker;
            OverloadBuffer = overloadBuffer;
            Handle = handle;

            Profiling.Profiler.Start(method, ProfilerMode.Manual);

            Profiler = Profiling.Profiler.GetRecord(method);

            _allCallers.Add(this);
        }

        public object InvokeInternalBuffer(Action<object[]> setBuffer = null)
        {
            if (OverloadBuffer != null)
            {
                var bufLength = OverloadBuffer?.Length ?? -1;

                setBuffer.SafeCall(OverloadBuffer);

                var newBufLength = OverloadBuffer?.Length ?? -1;

                if (bufLength != newBufLength)
                    throw new InvalidOperationException($"You cannot change the size of the buffer while invoking a method.");
            }

            var frameStartValue = Profiler.TryNewFrame(out var frame);
            var result = Invoker.SafeCall(Handle, OverloadBuffer);

            if (IsProfilerEnabled && frameStartValue)
                Profiler.EndFrame(frame);

            if (OverloadBuffer != null)
                Array.Clear(OverloadBuffer, 0, OverloadBuffer.Length);

            return result;
        }

        public TResult InvokeInternalBuffer<TResult>(Action<object[]> setBuffer = null)
        {
            var result = InvokeInternalBuffer(setBuffer);

            if (result is null)
                return default;

            if (result is not TResult tResult)
                throw new InvalidOperationException($"Method '{Method.ToName()}' returned a different return value than expected ({typeof(TResult).ToName()})");

            return tResult;
        }

        public object Invoke(params object[] args)
        {
            if (OverloadBuffer != null && args.Length != OverloadBuffer.Length)
                throw new InvalidOperationException($"Invalid amount of parameters for method '{Method.ToName()}'");

            if (OverloadBuffer != null)
                Array.Copy(args, OverloadBuffer, args.Length);

            var frameStartValue = Profiler.TryNewFrame(out var frame);
            var result = Invoker.SafeCall(Handle, OverloadBuffer);

            if (IsProfilerEnabled && frameStartValue)
                Profiler.EndFrame(frame);

            if (OverloadBuffer != null)
                Array.Clear(OverloadBuffer, 0, OverloadBuffer.Length);

            return result;
        }

        public TResult Invoke<TResult>(params object[] args)
        {
            var result = Invoke(args);

            if (result is null)
                return default;

            if (result is not TResult tResult)
                throw new InvalidOperationException($"Method '{Method.ToName()}' returned a different return value than expected ({typeof(TResult).ToName()})");

            return tResult;
        }

        public static CallInfo Get(MethodInfo target, object handle, CallFlags flags, Delay delay)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            if (!target.IsStatic && handle is null)
                throw new ArgumentNullException(nameof(handle));

            if (!target.IsStatic && handle.GetType() != target.DeclaringType)
                throw new InvalidOperationException($"Provided handle type '{handle.GetType().ToName()}' is invalid for method '{target.ToName()}'");

            for (int i = 0; i < _allCallers.Count; i++)
            {
                if (_allCallers[i].Method == target && ObjectUtilities.IsInstance(handle, _allCallers[i].Handle))
                    return _allCallers[i];
            }

            var invoker = target.GetFastInvoker();

            if (invoker is null)
                throw new Exception($"Failed to create a call invoker.");

            var parameters = target.GetParameters();

            object[] buffer = null;

            if (parameters.Length > 0)
                buffer = new object[parameters.Length];

            return new CallInfo(target, invoker, buffer, handle, flags, delay);
        }
    }
}