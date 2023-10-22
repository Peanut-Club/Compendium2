using System;

namespace Compendium.Commands.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class CommandArgumentRestrictionAttribute : Attribute
    {
        public CommandArgumentRestrictionInfo[] Restrictions { get; }

        public CommandArgumentRestrictionAttribute(params CommandArgumentRestrictionInfo[] restrictions)
            => Restrictions = restrictions;
    }
}