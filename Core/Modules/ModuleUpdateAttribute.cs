using Compendium.Utilities;

using System;

namespace Compendium.Modules
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ModuleUpdateAttribute : Attribute
    {
        public bool IsProfiled { get; set; }
        public bool IsInherited { get; set; }

        public ModuleUpdateSource Source { get; set; }

        public Delay Delay { get; set; }
    }
}