using Compendium.Attributes;
using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Patching
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PatchAttribute : ResolveableAttribute<PatchAttribute>
    {
        private MethodInfo target;
        private PatchFlags flags;

        internal Type eventType;

        public PatchInfo Info { get; private set; }

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

        public override void OnResolved(AttributeInfo<PatchAttribute> attributeInfo)
        {
            base.OnResolved(attributeInfo);

            if (attributeInfo.Location != AttributeLocation.Method || attributeInfo.Method is null)
                return;

            if (target is null)
                throw new InvalidOperationException($"Attribute is not valid on method '{attributeInfo.Method.ToName()}'");

            Info = new PatchInfo(Patcher.Instance, $"{target.ToName()} -> {attributeInfo.Method.ToName()} ({flags})", flags, target, attributeInfo.Method);
        }
    }
}