using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Commands.Arguments
{
    public class CommandArgumentParserInfo
    {
        public Type Type { get; }

        public string Description { get; }
        public string[] Values { get; }

        public Func<CommandArgumentParserData, CommandArgumentParserResult> Parser { get; }

        public CommandArgumentParserInfo(Type type, string description, string[] values, Func<CommandArgumentParserData, CommandArgumentParserResult> parser)
        {
            Type = type;
            Description = description;
            Values = values;
            Parser = parser;
        }

        public bool TryParse(CommandArgumentParserData data, out CommandArgumentParserResult value)
        {
            if (Parser is null)
                throw new InvalidOperationException($"This argument's parser is invalid.");

            value = Parser.SafeCall(data);

            return value.IsParsed;
        }
    }
}
