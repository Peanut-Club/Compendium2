using Common.Extensions;

using System;
using System.Reflection;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventPatchAttribute : Attribute
    {
        public MethodInfo Patch { get; }

        public EventPatchAttribute(Type type, string name)
            => Patch = type.Method(name);
    }
}