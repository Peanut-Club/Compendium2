using Compendium.Commands.Arguments;
using Compendium.Profiling;
using Compendium.Utilities;
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

        public bool IsExecuting { get; private set; }
        public bool IsProfiled { get => Caller.Flags.Any(CallFlags.EnableProfiler); set => Caller.Flags = value ? Caller.Flags | CallFlags.EnableProfiler : Caller.Flags & ~CallFlags.EnableProfiler; }

        public bool IgnoreExtraArguments { get; }

        public int Priority { get; }

        public CommandSource Source { get; }
        public CommandArgumentInfo[] Arguments { get; }
        public CallInfo Caller { get; }

        public ProfilerRecord Profiler => Caller.Profiler;

        public CommandInfo(string name, string description, object handle, int priority, bool ignoreExtra, CommandSource source, CommandArgumentInfo[] arguments, CallInfo caller)
        {
            Name = name;
            Description = description;
            Handle = handle;
            Source = source;
            Arguments = arguments;
            Caller = caller;
            Priority = priority;
            IgnoreExtraArguments = ignoreExtra;
        }

        public CommandResult? Invoke(object[] args)
        {
            if (IsExecuting)
                return null;

            IsExecuting = true;

            try
            {
                var result = Caller.InvokeUnsafe(args);

                IsExecuting = false;

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
                IsExecuting = false;
                return new CommandResult(ex, "An exception was caugh while attempting to execute this command.");
            }
        }
    }
}