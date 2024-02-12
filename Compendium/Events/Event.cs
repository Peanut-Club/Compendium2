using Common.Extensions;
using Common.IO.Collections;
using Common.Logging;
using Common.Pooling.Pools;

using System;
using System.Reflection;
using System.Collections.Generic;

namespace Compendium.Events
{
    public class Event 
    {
        public static LogOutput Log => EventManager.Log;

        private static readonly LockedDictionary<Type, PropertyInfo[]> properties = new LockedDictionary<Type, PropertyInfo[]>();

        private Dictionary<string, object> values;
        private Dictionary<string, PropertyInfo> props;

        private EventMethod previousHandler;
        private EventMethod currentHandler;
        private EventMethod nextHandler;

        public virtual bool IsCancellable { get; }

        public EventMethod PreviousHandler => previousHandler;
        public EventMethod CurrentHandler => currentHandler;
        public EventMethod NextHandler => nextHandler;

        public Event()
        {
            if (!properties.ContainsKey(GetType()))
            {
                var props = new List<PropertyInfo>();

                foreach (var prop in GetType().GetAllProperties())
                {
                    var attribute = prop.GetCustomAttribute<EventPropertyAttribute>();

                    if (attribute != null)
                        props.Add(prop);
                }

                properties[GetType()] = props.ToArray();
            }
        }

        internal void GenerateArgs(ParameterInfo[] parameters, object[] args)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (values.TryGetValue(parameters[i].Name, out var value))
                    args[i] = value;
                else
                    args[i] = null;
            }
        }

        internal void FinishArgs(object[] args, ParameterInfo[] parameters)
        {
            if (props is null)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!parameters[i].ParameterType.IsByRef)
                    continue;

                if (!props.TryGetValue(parameters[i].Name, out var prop))
                    continue;

                prop.SetValueFast(args[i], this);
            }
        }

        internal void SetProperties()
        {
            if (!properties.TryGetValue(GetType(), out var evProperties))
                return;

            values = DictionaryPool<string, object>.Shared.Rent();
            props = DictionaryPool<string, PropertyInfo>.Shared.Rent();

            for (int i = 0; i < evProperties.Length; i++)
            {
                var value = evProperties[i].GetValueFast<object>(this);

                values[evProperties[i].Name] = value;
                props[evProperties[i].Name] = evProperties[i];
            }
        }

        internal void ReturnProperties()
        {
            if (values != null)
            {
                DictionaryPool<string, object>.Shared.Return(values);
                values = null;
            }

            if (props != null)
            {
                DictionaryPool<string, PropertyInfo>.Shared.Return(props);
                props = null;
            }
        }

        internal void UpdateHandlers(EventMethod previous, EventMethod current, EventMethod next)
        {
            previousHandler = previous;
            currentHandler = current;
            nextHandler = next;
        }
    }
}