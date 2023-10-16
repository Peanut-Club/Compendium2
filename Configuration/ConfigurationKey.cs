using Compendium.Utilities.Reflection;

using System.Reflection;

namespace Compendium.Configuration
{
    public class ConfigurationKey
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public Assembly Assembly { get; set; }
        public MemberInfo Member { get; set; }

        public object Handle { get; set; }

        public ConfigurationCallbackPair Callbacks { get; } = new ConfigurationCallbackPair();

        public void Set(object value)
            => Callbacks.Setter.SafeCall(Handle, value);

        public object Get()
            => Callbacks.Getter.SafeCall(Handle);
    }
}