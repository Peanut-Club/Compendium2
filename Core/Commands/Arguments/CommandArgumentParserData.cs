using System;

namespace Compendium.Commands.Arguments
{
    public struct CommandArgumentParserData
    {
        public readonly CommandContext Context;
        public readonly ArraySegment<string> Args;
        public readonly string Value;
        public readonly int Position;

        public CommandArgumentParserData(CommandContext ctx, ArraySegment<string> args, string value, int pos)
        {
            Context = ctx;
            Args = args;
            Value = value;
            Position = pos;
        }
    }
}