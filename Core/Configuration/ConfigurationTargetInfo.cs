using System;
using System.Reflection;

namespace Compendium.Configuration
{
    public class ConfigurationTargetInfo
    {
        public Type ValueType { get; }

        public MemberInfo Target { get; }

        public object Handle { get; }

        public object Value
        {
            get => ConfigurationUtils.GetValue(Handle, Target);
            set => ConfigurationUtils.SetValue(Handle, value, Target);
        }

        public string Name { get; }
        public string Description { get; }

        public ConfigurationTargetStatus Status { get; internal set; } = ConfigurationTargetStatus.None;

        public ConfigurationTargetInfo(Type valueType, MemberInfo target, object handle, string name, string description)
        {
            ValueType = valueType;
            Target = target;
            Handle = handle;
            Name = name;
            Description = description;
        }
    }
}