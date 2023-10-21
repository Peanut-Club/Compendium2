using System;

namespace Compendium.Commands.Arguments
{
    public struct CommandArgumentParserResult
    {
        public readonly bool IsParsed;
        public readonly object Value;
        public readonly string Message;

        public readonly ArraySegment<string>? NewArgs;
        public readonly int? NewPos;

        public CommandArgumentParserResult(bool parsed, object value, string message, ArraySegment<string>? newArgs = null, int? newPos = null)
        {
            IsParsed = parsed;
            Value = value;
            Message = message;
            NewArgs = newArgs;
            NewPos = newPos;
        }
    }
}