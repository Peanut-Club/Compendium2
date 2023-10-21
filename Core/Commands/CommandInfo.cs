using Compendium.Commands.Arguments;
using Compendium.Utilities.Calls;

using System;
using System.Collections;
using System.Text;

namespace Compendium.Commands
{
    public class CommandInfo
    {
        public string Name { get; }
        public string Description { get; }

        public object Handle { get; }
        public object[] Buffer { get; }

        public bool IsExecuting { get; private set; }

        public CommandInfo Parent { get; }
        public CommandSource Source { get; }

        public CommandArgumentInfo[] Arguments { get; }

        public CallInfo Caller { get; }

        public CommandInfo(string name, string description, object handle, object[] buffer, CommandInfo parent, CommandSource source, CommandArgumentInfo[] arguments, CallInfo caller)
        {
            Name = name;
            Description = description;
            Handle = handle;
            Buffer = buffer;
            Parent = parent;
            Source = source;
            Arguments = arguments;
            Caller = caller;
        }

        public CommandResult? Invoke(CommandContext ctx, string args)
        {
            if (IsExecuting)
                return null;

            IsExecuting = true;

            if (Arguments.Length > 0)
            {
                var parserResult = CommandArgumentHandler.ParseArguments(this, ctx, args);

                if (!parserResult.IsSuccess)
                {
                    IsExecuting = false;
                    
                    return new CommandResult(false, false, $"Argument Parser failed at argument '{Arguments[parserResult.FaultyArgumentPosition].Name}'     \n{parserResult.FaultMessage}", null);
                }
            }

            try
            {
                var result = Caller.InvokeUnsafe(Buffer);

                if (result is null)
                    return new CommandResult(true, true, null, null);
                else if (result is CommandResult commandResult)
                    return commandResult;
                else if (result is string str)
                    return new CommandResult(str);
                else if (result is bool success)
                    return new CommandResult(success, !success, null, null);
                else if (result is IEnumerable objects)
                {
                    var rStr = "";

                    foreach (var obj in objects)
                    {
                        try
                        {
                            rStr += $"\n{obj ?? "null object"}";
                        }
                        catch (Exception ex)
                        {
                            rStr += $"\n= Exception thrown for this object: {ex.Message} =";
                        }
                    }

                    return new CommandResult(rStr);
                }
                else if (result is StringBuilder builder)
                    return new CommandResult(builder);
                else
                    return new CommandResult(true, true, $"Unsupported return value type: {result.GetType().FullName}", null);
            }
            catch (Exception ex)
            {
                return new CommandResult(ex, "An exception was caugh while attempting to execute this command.");
            }
        }
    }
}