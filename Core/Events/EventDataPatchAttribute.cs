using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventDataPatchAttribute : Attribute
    {
        public Type Type { get; }

        public EventDataPatchAttribute(Type type)
        {
            Type = type;
        }
    }
}