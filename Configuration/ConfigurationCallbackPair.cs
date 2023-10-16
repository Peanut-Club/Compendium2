using System;

namespace Compendium.Configuration
{
    public class ConfigurationCallbackPair
    {
        public Action<object, object> Setter { get; set; }
        public Func<object, object> Getter { get; set; }
    }
}