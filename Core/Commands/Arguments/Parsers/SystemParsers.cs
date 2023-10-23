using Compendium.Logging;
using Compendium.Results;
using Compendium.Utilities;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;

namespace Compendium.Commands.Arguments.Parsers
{
    public static class SystemParsers
    {
        private static readonly Type[] _caseSensitiveArgs;
        private static readonly Type[] _normalArgs;
        private static readonly Type[] _parsedTypes;

        private static readonly FastDelegate[] _parserDelegates;

        private static readonly bool[] _parserArgs;

        private static readonly Dictionary<Type, Func<string, bool, IResult>> _parsers = new Dictionary<Type, Func<string, bool, IResult>>();

        static SystemParsers()
        {
            _parsedTypes = new Type[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(decimal),
                typeof(float),

                typeof(string),

                typeof(bool),

                typeof(DateTime),
                typeof(TimeSpan)
            };

            _caseSensitiveArgs = new Type[] { typeof(string), typeof(bool) };
            _normalArgs = new Type[] { typeof(string) };

            _parserDelegates = new FastDelegate[_parsedTypes.Length];
            _parserArgs = new bool[_parsedTypes.Length];

            for (int i = 0; i < _parsedTypes.Length; i++)
            {
                var parseMethod = _parsedTypes[i].GetMethod("Parse", MethodUtilities.BindingFlags, null, _caseSensitiveArgs, null);

                if (parseMethod is null)
                {
                    parseMethod = _parsedTypes[i].GetMethod("Parse", MethodUtilities.BindingFlags, null, _normalArgs, null);
                    _parserArgs[i] = false;
                }
                else
                    _parserArgs[i] = true;

                if (parseMethod is null)
                {
                    Log.Critical("System Parsers", $"Type '{_parsedTypes[i].FullName}' does not have a 'Parse' method.");
                    continue;
                }

                var parseDelegate = parseMethod.GetFastInvoker(true);

                if (parseDelegate is null)
                {
                    Log.Critical("System Parsers", $"Failed to create parser delegate for type '{_parsedTypes[i].FullName}' ({parseMethod.ToName()})");
                    continue;
                }

                _parserDelegates[i] = parseDelegate;

                _parsers[_parsedTypes[i]] = (str, sensitive) =>
                {
                    var parser = _parserDelegates[i];

                    try
                    {
                        return ResultUtils.Success(parser(null, _parserArgs[i] ? new object[] { str, sensitive } : new object[] { str }));
                    }
                    catch (Exception ex)
                    {
                        return ResultUtils.Error(ex);
                    }
                };
            }
        }

        [CommandArgumentParser(typeof(byte))]
        [CommandArgumentParser(typeof(sbyte))]
        [CommandArgumentParser(typeof(short))]
        [CommandArgumentParser(typeof(ushort))]
        [CommandArgumentParser(typeof(int))]
        [CommandArgumentParser(typeof(uint))]
        [CommandArgumentParser(typeof(long))]
        [CommandArgumentParser(typeof(ulong))]
        [CommandArgumentParser(typeof(decimal))]
        [CommandArgumentParser(typeof(float))]

        [CommandArgumentParser(typeof(string))]

        [CommandArgumentParser(typeof(bool))]

        [CommandArgumentParser(typeof(DateTime))]
        [CommandArgumentParser(typeof(TimeSpan))]
        public static IResult Parse(CommandContext ctx, Type type, string value)
        {
            if (_parsers.TryGetValue(type, out var parser))
                return parser.SafeCall(value, ctx.Source.IsCaseSensitive);
            else
                return ResultUtils.Error($"There are no simple parsers for type '{type.ToName()}'");
        }

        public static void ReplaceParser<T>(Func<string, bool, IResult> parser)
            => _parsers[typeof(T)] = parser;
    }
}