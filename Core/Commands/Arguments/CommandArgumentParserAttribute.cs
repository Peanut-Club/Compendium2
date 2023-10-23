using System;

namespace Compendium.Commands.Arguments
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandArgumentParserAttribute : Attribute
    {
        public Type Type { get; }

        public string Description { get; }
        public string[] Values { get; }

        public CommandArgumentParserAttribute(Type type, string description, params string[] values)
        {
            Type = type;
            Description = description;
            Values = values;
        }

        public CommandArgumentParserAttribute(Type type) : this(type, "No description.", Array.Empty<string>()) { }
    }
}