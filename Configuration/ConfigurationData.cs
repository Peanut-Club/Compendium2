using System;
using System.Collections.Generic;

namespace Compendium.Configuration
{
    public class ConfigurationData
    {
        public string Path { get; set; }
        public string Id { get; set; }

        public List<ConfigurationKey> Keys { get; } = new List<ConfigurationKey>();
        public List<Action> ReloadHandlers { get; } = new List<Action>();
    }
}
