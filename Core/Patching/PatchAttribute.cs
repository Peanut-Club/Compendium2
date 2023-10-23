using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Patching
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PatchAttribute : Attribute
    {
        internal MethodInfo target;
        internal PatchFlags flags;
        internal Type eventType;

        public PatchDescriptor Info { get; private set; }

        public PatchAttribute(MethodInfo target, PatchFlags flags)
        {
            this.target = target;
            this.flags = flags;
        }

        public PatchAttribute(MethodInfo target, PatchFlags flags, Type eventType)
        {
            this.target = target;
            this.flags = flags;
            this.eventType = eventType;
        }

        public PatchAttribute(Type type, string name, PatchFlags flags) : this(type.Method(name), flags) { }
        public PatchAttribute(Type type, string name, PatchFlags flags, params Type[] overload) : this(type.Method(name, overload), flags) { }
        public PatchAttribute(Type type, string name, Type genericType, PatchFlags flags, params Type[] overload) : this(type.Method(name, genericType, overload), flags) { }
    }
}