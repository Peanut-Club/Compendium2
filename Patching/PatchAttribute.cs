using Compendium.Attributes;
using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Patching
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PatchAttribute : ResolveableAttribute<PatchAttribute>
    {
        public MethodInfo Target { get; }
        public MethodInfo Replacement { get; private set; }

        public MethodInfo Patch { get; internal set; }

        public PatchTiming Timing { get; } = PatchTiming.BeforeExecution;
        public PatchType Type { get; } = PatchType.Method;

        public PatchAttribute(Type type, string name, PatchTiming timing = PatchTiming.BeforeExecution, PatchType patchType = PatchType.Method)
        {
            Target = type.Method(name);

            Timing = timing;
            Type = patchType;

            if (Target is null)
                throw new MissingMethodException($"Failed to find a method of name '{name}' in type '{type.FullName}'");
        }

        public PatchAttribute(PatchTargetInfo info, PatchTiming timing = PatchTiming.BeforeExecution, PatchType type = PatchType.Method)
        {
            Target = info.Resolve(true);

            if (Target is null)
                throw new MissingMethodException($"Failed to find a method of name '{info.TargetName}' in type '{info.TargetType.FullName}'");

            Timing = timing;
            Type = type;
        }

        public override void OnResolved(AttributeInfo<PatchAttribute> attributeInfo)
        {
            base.OnResolved(attributeInfo);
            Replacement = attributeInfo.Method;
        }
    }
}