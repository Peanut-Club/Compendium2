using Compendium.Commands.Arguments;
using Compendium.Extensions;
using Compendium.Pooling.Pools;
using Compendium.Results;
using Compendium.Utilities;
using Compendium.Utilities.Calls;
using Compendium.Utilities.Instances;
using Compendium.Utilities.Reflection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Commands
{
    public class CommandSource
    {
        public static readonly CommandSource RemoteAdmin = new CommandSource();
        public static readonly CommandSource Console = new CommandSource();
        public static readonly CommandSource Player = new CommandSource();

        private readonly List<CommandInfo> _commands = new List<CommandInfo>();

        public bool IsCaseSensitive { get; set; }

        public void AddCommands()
            => AddCommands(Assembly.GetCallingAssembly());

        public void AddCommands(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                AddCommands(type, InstanceTracker.Get(type));
        }

        public void AddCommands(Type type, object handle)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(handle));

            if (!ObjectUtilities.VerifyClassInstanceForMember(type, handle))
                throw new ArgumentException($"Invalid type handle");

            foreach (var method in type.GetAllMethods())
            {
                if (method.HasAttribute<CommandAttribute>(out var commandAttribute))
                {
                    if (string.IsNullOrWhiteSpace(commandAttribute.Name))
                        continue;

                    if (_commands.Any(cmd => StringMatch(cmd.Name, commandAttribute.Name)
                                && cmd.Arguments.Select(x => x.Type).IsMatch(method.GetParameters().Select(x => x.ParameterType))))
                        continue;

                    AddCommand(new CommandInfo(
                        commandAttribute.Name,
                        commandAttribute.Description,

                        handle,

                        (int)commandAttribute.Priority,

                        commandAttribute.IgnoreExtraArgs,

                        this,

                        GatherArguments(method),

                        CallInfo.Get(method, handle, CallFlags.None, null)));
                }
            }
        }

        public void AddCommand(CommandInfo command)
        {
            if (_commands.Contains(command) 
                || _commands.Any(cmd => StringMatch(cmd.Name, command.Name) 
                        && cmd.Arguments.Select(x => x.Type).IsMatch(command.Arguments.Select(x => x.Type))))
                return;

            _commands.Add(command);
        }

        public void RemoveCommand(CommandInfo command)
            => _commands.Remove(command);

        public void RemoveCommands(Type type, object handle)
            => _commands.RemoveAll(c => c.Caller.Method.DeclaringType == type && ObjectUtilities.IsInstance(handle, c.Handle));

        public void RemoveCommands(Assembly assembly)
            => _commands.RemoveAll(c => c.Caller.Method.DeclaringType.Assembly == assembly);

        public void RemoveCommands()
            => RemoveCommands(Assembly.GetCallingAssembly());

        public IResult Execute(string args, CommandContext ctx)
        {
            var searchResult = Search(args);

            if (!searchResult.TryReadValue<CommandInfo[]>(out var commands))
                return ResultUtils.Error("$ID_NoMatching");

            var validCommands = new List<Tuple<CommandInfo, IResult>>();

            for (int i = 0; i < commands.Length; i++)
            {
                var parseResult = CommandArgumentHandler.Parse(commands[i], ctx, args.Substring(0, commands[i].Name.Length).Trim(), 0);

                if (!parseResult.IsSuccess)
                    continue;

                validCommands.Add(new Tuple<CommandInfo, IResult>(commands[i], parseResult));
            }

            if (validCommands.Count <= 0)
                return ResultUtils.Error("$ID_NoMatching_Parsing");

            var cmds = validCommands.OrderByDescending(t => t.Item1.Priority);
            var chosenCmd = cmds.First();
            var parsingResult = ApplyParsedResult(chosenCmd.Item2, chosenCmd.Item1);

            if (!parsingResult.TryReadValue<object[]>(out var overloadArgs))
                return parsingResult;

            var exResult = chosenCmd.Item1.Invoke(overloadArgs).GetValueOrDefault();

            return exResult.IsSuccess 
                ? ResultUtils.Success(exResult) 
                : ResultUtils.Error(exResult.Response, exResult.Exception);
        }

        public IResult Search(string args)
        {
            var cmdList = ListPool<CommandInfo>.Shared.Rent();

            for (int i = 0; i < _commands.Count; i++)
            {
                if (args.StartsWith(_commands[i].Name) 
                    || (!IsCaseSensitive && args.ToLowerInvariant().StartsWith(_commands[i].Name.ToLowerInvariant())))
                    cmdList.Add(_commands[i]);
            }

            if (cmdList.Count > 0)
                return ResultUtils.Success(ListPool<CommandInfo>.Shared.ToArrayReturn(cmdList));
            else
            {
                ListPool<CommandInfo>.Shared.Return(cmdList);
                return ResultUtils.Error();
            }
        }

        public bool StringMatch(string str1, string str2)
        {
            if (IsCaseSensitive)
                return str1 == str2;
            else
                return str1.ToLowerInvariant() == str2.ToLowerInvariant();
        }

        private IResult ApplyParsedResult(IResult parsedResult, CommandInfo command)
        {
            if (!parsedResult.TryReadValue<Tuple<IResult[], IResult[]>>(out var tuple))
                return ResultUtils.Error();

            var argArray = tuple.Item1;
            var paramArray = tuple.Item2;

            var argList = new object[argArray.Length];
            var paramList = new object[paramArray.Length];

            for (int i = 0; i < argArray.Length; i++)
            {
                if (!argArray[i].IsSuccess)
                    return argArray[i];

                argList[i] = argArray[i].Result;
            }

            for (int i = 0; i < paramArray.Length; i++)
            {
                if (!paramArray[i].IsSuccess)
                    return paramArray[i];

                paramList[i] = paramArray[i].Result;
            }

            var argsCount = command.Arguments.Length;
            var array = new object[command.Arguments.Length];
            var hasVarArgs = command.Arguments.Last().Options.IsCatchAll;

            if (hasVarArgs)
                argsCount--;

            var x = 0;

            foreach (var arg in argList)
            {
                if (x == argsCount)
                    return ResultUtils.Error("Command was invoked with too many parameters.");

                array[x++] = arg;
            }

            if (x < argsCount)
                return ResultUtils.Error("Command was invoked with too few parameters.");

            if (hasVarArgs)
            {
                var func = CommandArgumentHandler.GetOrAddArrayConverter(command.Arguments[command.Arguments.Length - 1].Type, t =>
                {
                    var method = CommandArgumentHandler._convertParamsMethod.MakeGenericMethod(t);

                    return (Func<IEnumerable<object>, object>)method.CreateDelegate(typeof(Func<IEnumerable<object>, object>));
                });

                array[x] = func.SafeCall(paramList);
            }

            return ResultUtils.Success(array);
        }

        private CommandArgumentInfo[] GatherArguments(MethodInfo method)
        {
            var mParams = method.GetParameters();

            if (mParams.Length <= 0)
                return Array.Empty<CommandArgumentInfo>();

            var args = ListPool<CommandArgumentInfo>.Shared.Rent();

            for (int i = 0; i < mParams.Length; i++)
            {
                var options = new CommandArgumentOptionsInfo();
                var castAttribute = mParams[i].GetCustomAttribute<CommandCastAttribute>();
                var descAttribute = mParams[i].GetCustomAttribute<CommandArgumentDescriptionAttribute>();
                var restrictionAttribute = mParams[i].GetCustomAttribute<CommandArgumentRestrictionAttribute>();

                if (mParams[i].IsOptional)
                    options.IsOptional = true;

                if (mParams[i].IsDefined(typeof(ParamArrayAttribute)))
                    options.IsCatchAll = true;

                if (mParams[i].IsDefined(typeof(CommandRemainderAttribute)))
                    options.IsRemainder = true;

                if (mParams[i].IsDefined(typeof(CommandCaseSensitiveAttribute)))
                    options.IsSensitive = true;

                if (castAttribute != null)
                    options.IsCast = true;

                if (mParams[i].ParameterType == typeof(CommandContext))
                    options.IsContext = true;

                if (mParams[i].ParameterType.InheritsType<Player>() && (i is 0 || mParams[i].Name == "sender"))
                    options.IsSender = true;

                args.Add(new CommandArgumentInfo(
                    mParams[i].ParameterType, 
                    mParams[i].Name, 
                    
                    descAttribute?.Description ?? "Default Description", 
                    
                    mParams[i].Position, 
                    mParams[i].DefaultValue, 
                    
                    options, 
                    
                    castAttribute?.Options ?? default, 
                    
                    CommandArgumentHandler.GetParser(mParams[i].ParameterType), 
                    
                    restrictionAttribute?.Restrictions ?? Array.Empty<CommandArgumentRestrictionInfo>(), 
                    
                    mParams[i].ParameterType.InheritsInterface<IEnumerable>(), 
                    mParams[i].ParameterType.InheritsInterface<IDictionary>(), 
                    mParams[i].ParameterType.IsArray));
            }

            return ListPool<CommandArgumentInfo>.Shared.ToArrayReturn(args);
        }
    }
}