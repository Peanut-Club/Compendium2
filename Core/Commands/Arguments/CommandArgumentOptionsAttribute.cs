using System;

namespace Compendium.Commands.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CommandArgumentOptionsAttribute : Attribute
    {
        public bool IsRemainder { get; set; }
        public bool IsCaseSensitive { get; set; }

        public CommandArgumentRestrictionInfo[] Restrictions { get; set; }
    }
}