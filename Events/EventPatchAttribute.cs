using Compendium.Patching;

using System;

namespace Compendium.Events
{
    public class EventPatchAttribute : PatchAttribute
    {
        public Type EventType { get; }

        public EventPatchAttribute(Type eventType, Type type, string name, PatchTiming timing = PatchTiming.BeforeExecution, PatchType patchType = PatchType.Method) : base(type, name, timing, patchType) 
        {
            EventType = eventType;
        }

        public EventPatchAttribute(Type eventType, PatchTargetInfo info, PatchTiming timing = PatchTiming.AfterExecution, PatchType patchType = PatchType.Method) : base(info, timing, patchType)
        {
            EventType = eventType;
        }
    }
}