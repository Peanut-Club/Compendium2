using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventPatchAttribute : Attribute
    {
        public Type Type { get; }

        public EventPatchAttribute(Type type)
        {
            Type = type;
        }
    }
}