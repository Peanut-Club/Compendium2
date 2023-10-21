using System;
using System.Collections.Generic;

namespace Compendium.Commands.Arguments
{
    public static class CommandArgumentHandler
    {
        private static readonly List<CommandArgumentParserInfo> _parsers = new List<CommandArgumentParserInfo>();

        public static CommandArgumentHandlerResult ParseArguments(CommandInfo command, CommandContext ctx, string args)
        {
            if (command.Arguments.Length <= 0)
                return new CommandArgumentHandlerResult(true, -1, null);

            
        }
    }
}