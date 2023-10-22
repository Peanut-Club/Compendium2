using System;

namespace Compendium.Commands.Arguments
{
    public class CommandArgumentInfo
    {
        public Type Type { get; }

        public string Name { get; }
        public string Description { get; }

        public int Position { get; }

        public object Default { get; }

        public CommandArgumentOptionsInfo Options { get; }
        public CommandArgumentCastOptions CastOptions { get; }

        public CommandArgumentParserInfo Parser { get; }
        public CommandArgumentRestrictionInfo[] Restrictions { get; }

        public bool IsEnumerable { get; }
        public bool IsDictionary { get; }
        public bool IsArray { get; }

        public CommandArgumentInfo(Type type, string name, string description, int position, object @default, CommandArgumentOptionsInfo options, CommandArgumentCastOptions castOptions, CommandArgumentParserInfo parser, CommandArgumentRestrictionInfo[] restrictions, bool isEnumerable, bool isDictionary, bool isArray)
        {
            Type = type;
            Name = name;
            Description = description;
            Position = position;
            Default = @default;
            Options = options;
            CastOptions = castOptions;
            Parser = parser;
            Restrictions = restrictions;
            IsEnumerable = isEnumerable;
            IsDictionary = isDictionary;
            IsArray = isArray;
        }
    }
}
