using System;

namespace Compendium.Commands.Arguments
{
    public class CommandArgumentInfo
    {
        public Type Type { get; }

        public string Name { get; }
        public string Description { get; }

        public int Position { get; }

        public CommandArgumentFlags Flags { get; } = CommandArgumentFlags.None;

        public CommandArgumentParserInfo Parser { get; }
        public CommandArgumentRestrictionInfo[] Restrictions { get; }

        public CommandArgumentInfo(Type type, string name, string description, int position, CommandArgumentFlags flags, CommandArgumentParserInfo parser, CommandArgumentRestrictionInfo[] restrictions)
        {
            Type = type;
            Name = name;
            Description = description;
            Position = position;
            Flags = flags;
            Parser = parser;
            Restrictions = restrictions;
        }
    }
}
