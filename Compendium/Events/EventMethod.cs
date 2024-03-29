﻿using Common.Extensions;
using Common.Logging;

using System;
using System.Reflection;

namespace Compendium.Events
{
    public class EventMethod
    {
        private object[] argsCache;

        public static LogOutput Log => EventManager.Log;

        public MethodInfo Target { get; }
        public EventRecord Record { get; }

        public ParameterInfo[] Parameters { get; }

        public object Instance { get; }

        public bool IsStatic
        {
            get => Target.IsStatic;
        }

        public bool IsValid
        {
            get => Target != null && (IsStatic || Instance != null);
        }

        public bool IsEventParam { get; }
        public bool IsLess { get; }

        public bool IsRecording { get; set; }

        public EventMethod(MethodInfo target, object instance, bool isRecording)
        {
            Target = target;
            Instance = instance;

            Parameters = target.Parameters();

            IsRecording = isRecording;
            IsLess = Parameters.Length == 0;

            if (!IsLess)
            {
                IsEventParam = Parameters.Length == 1 && Parameters[0].ParameterType.IsSubclassOf(typeof(Event));

                argsCache = new object[Parameters.Length];
            }

            Record = new EventRecord();
        }

        public object Invoke(Event ev)
        {
            if (IsRecording)
            {
                var time = DateTime.Now;
                var result = InternalInvoke(ev, out var isError);
                var duration = (float)(DateTime.Now - time).TotalMilliseconds;

                Record.Update(duration, !isError);

                return result;
            }
            else
            {
                return InternalInvoke(ev, out _);
            }
        }

        private object InternalInvoke(Event ev, out bool isError)
        {
            if (IsLess)
            {
                return CallWithArgs(null, ev, out isError);
            }
            else if (IsEventParam)
            {
                argsCache[0] = ev;
                return CallWithArgs(argsCache, ev, out isError);
            }
            else
            {
                ev.GenerateArgs(Parameters, argsCache);

                var result = CallWithArgs(argsCache, ev, out isError);

                ev.FinishArgs(argsCache, Parameters);

                return result;
            }
        }

        private object CallWithArgs(object[] args, Event ev, out bool isError)
        {
            try
            {
                isError = false;

                return Target.CallUnsafe(Instance, args);
            }
            catch (Exception ex)
            {
                isError = true;

                Log.Error($"Method '{Target.ToName()}' failed to handle event '{ev.GetType().Name}':\n{ex}");

                return null;
            }
        }
    }
}