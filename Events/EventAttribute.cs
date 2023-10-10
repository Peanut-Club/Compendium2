using Compendium.Attributes;
using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventAttribute : ResolveableAttribute<EventAttribute>
    {
        public Type Type { get; private set; }

        public EventAttribute() { }
        public EventAttribute(Type type)
            => Type = type;

        public override void OnResolved(AttributeInfo<EventAttribute> attributeInfo)
        {
            base.OnResolved(attributeInfo);

            if (attributeInfo.Location != AttributeLocation.Method || attributeInfo.Method is null)
                return;

            if (Type != null)
            {
                Type.VerifyEventType();
                return;
            }

            var methodArgs = attributeInfo.Method.GetParameters();

            if (methodArgs is null || methodArgs.Length < 1)
                throw new InvalidOperationException($"Invalid method parameters in method '{attributeInfo.Method.ToName()}'");

            var evArg = methodArgs[0].ParameterType;

            evArg.VerifyEventType();

            foreach (var ev in EventManager.Events)
            {
                if (ev.Type != null && ev.Type == evArg)
                {
                    Type = ev.Type;
                    break;
                }
            }

            if (Type is null)
                throw new InvalidOperationException($"Method parameter '{methodArgs[0].Name}' ({evArg.ToName()}) is not a valid EventInfo parameter.");
        }
    }
}