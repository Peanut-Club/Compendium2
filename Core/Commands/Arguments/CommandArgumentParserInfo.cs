using Compendium.Results;
using Compendium.Utilities;

using System;

namespace Compendium.Commands.Arguments
{
    public class CommandArgumentParserInfo
    {
        public Type Type { get; }

        public string Description { get; }
        public string[] Values { get; }

        public Func<CommandContext, Type, string, IResult> Parser { get; }

        public CommandArgumentParserInfo(Type type, string description, string[] values, Func<CommandContext, Type, string, IResult> parser)
        {
            Type = type;
            Description = description;
            Values = values;
            Parser = parser;
        }

        public IResult Parse(CommandContext ctx, string value)
        {
            if (Parser is null)
                throw new InvalidOperationException($"This argument's parser is invalid.");

            try
            {
                return Parser(ctx, Type, value);
            }
            catch (Exception ex)
            {
                return ResultUtils.Error(ex.Message, ex);
            }
        }
    }
}
