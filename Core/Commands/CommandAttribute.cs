using Compendium.Enums;

using System;

namespace Compendium.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; } = "No description.";

        public bool IgnoreExtraArgs { get; set; } = true;

        public Priority Priority { get; set; } = Priority.Normal;
    }
}