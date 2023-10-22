using Compendium.Pooling.Pools;
using Compendium.Results;
using Compendium.Utilities;
using Compendium.Utilities.Reflection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Compendium.Commands.Arguments
{
    public static class CommandArgumentHandler
    {
        internal static readonly MethodInfo _convertParamsMethod = typeof(CommandArgumentHandler).GetTypeInfo().GetDeclaredMethod("ConvertParamsList");
        
        private static readonly CommandArgumentProxyTypeCache _typeCache = new CommandArgumentProxyTypeCache();
        private static readonly List<CommandArgumentParserInfo> _parsers = new List<CommandArgumentParserInfo>();
        private static readonly Dictionary<Type, Func<IEnumerable<object>, object>> _arrayConverters = new Dictionary<Type, Func<IEnumerable<object>, object>>();

        public static readonly Dictionary<char, char> QuotationMarks = new Dictionary<char, char>
        {
                    {'\"', '\"' },
                    {'«', '»' },
                    {'‘', '’' },
                    {'“', '”' },
                    {'„', '‟' },
                    {'‹', '›' },
                    {'‚', '‛' },
                    {'《', '》' },
                    {'〈', '〉' },
                    {'「', '」' },
                    {'『', '』' },
                    {'〝', '〞' },
                    {'﹁', '﹂' },
                    {'﹃', '﹄' },
                    {'＂', '＂' },
                    {'＇', '＇' },
                    {'｢', '｣' },
                    {'(', ')' },
                    {'༺', '༻' },
                    {'༼', '༽' },
                    {'᚛', '᚜' },
                    {'⁅', '⁆' },
                    {'⌈', '⌉' },
                    {'⌊', '⌋' },
                    {'❨', '❩' },
                    {'❪', '❫' },
                    {'❬', '❭' },
                    {'❮', '❯' },
                    {'❰', '❱' },
                    {'❲', '❳' },
                    {'❴', '❵' },
                    {'⟅', '⟆' },
                    {'⟦', '⟧' },
                    {'⟨', '⟩' },
                    {'⟪', '⟫' },
                    {'⟬', '⟭' },
                    {'⟮', '⟯' },
                    {'⦃', '⦄' },
                    {'⦅', '⦆' },
                    {'⦇', '⦈' },
                    {'⦉', '⦊' },
                    {'⦋', '⦌' },
                    {'⦍', '⦎' },
                    {'⦏', '⦐' },
                    {'⦑', '⦒' },
                    {'⦓', '⦔' },
                    {'⦕', '⦖' },
                    {'⦗', '⦘' },
                    {'⧘', '⧙' },
                    {'⧚', '⧛' },
                    {'⧼', '⧽' },
                    {'⸂', '⸃' },
                    {'⸄', '⸅' },
                    {'⸉', '⸊' },
                    {'⸌', '⸍' },
                    {'⸜', '⸝' },
                    {'⸠', '⸡' },
                    {'⸢', '⸣' },
                    {'⸤', '⸥' },
                    {'⸦', '⸧' },
                    {'⸨', '⸩' },
                    {'【', '】'},
                    {'〔', '〕' },
                    {'〖', '〗' },
                    {'〘', '〙' },
                    {'〚', '〛' }
        };

        public static CommandArgumentParserInfo GetParser(Type type)
        {
            if (!_typeCache.IsContained(ref type))
            {
                if (type.IsArray)
                    _typeCache.Add(type, typeof(Array));
                else if (type.IsEnum)
                    _typeCache.Add(type, typeof(Enum));
                else if (type == typeof(string))
                    _typeCache.Add(type, typeof(string));
                else if (type.InheritsInterface<IDictionary>())
                    _typeCache.Add(type, typeof(IDictionary));
                else if (type.InheritsInterface<IEnumerable>())
                    _typeCache.Add(type, typeof(IEnumerable));

                _typeCache.IsContained(ref type);
            }

            for (int i = 0; i < _parsers.Count; i++)
            {
                if (_parsers[i].Type == type)
                    return _parsers[i];
            }

            return null;
        }

        public static Func<IEnumerable<object>, object> GetOrAddArrayConverter(Type type, Func<Type, Func<IEnumerable<object>, object>> func)
        {
            if (_arrayConverters.TryGetValue(type, out var converter))
                return converter;

            return _arrayConverters[type] = func.SafeCall(type);
        }

        public static IResult Parse(CommandInfo command, CommandContext ctx, string args, int pos)
        {
            CommandArgumentInfo arg = null;

            var builder = StringBuilderPool.Shared.Rent();
            var endPos = args.Length;
            var curPart = byte.MinValue;
            var lastEndPos = int.MinValue;
            var results = ListPool<IResult>.Shared.Rent();
            var paramResults = ListPool<IResult>.Shared.Rent();
            var isEscaping = false;
            var c = '\0';
            var match = '\0';

            bool IsOpenQuote(char ch)
                => QuotationMarks.ContainsKey(ch);

            char GetMatch(char ch)
                => QuotationMarks.TryGetValue(ch, out var value) ? value : '\"';

            for (int curPos = pos; curPos <= endPos; curPos++)
            {
                if (curPos < endPos)
                    c = args[curPos];
                else
                    c = '\0';

                if (arg != null && arg.Options.IsRemainder && curPos != endPos)
                {
                    builder.Append(c);
                    continue;
                }

                if (isEscaping)
                {
                    if (curPos != endPos)
                    {
                        if (c != match)
                            builder.Append('\\');

                        builder.Append(c);
                        isEscaping = false;
                        continue;
                    }
                }

                if (c == '\\' && (arg == null || !arg.Options.IsRemainder))
                {
                    isEscaping = true;
                    continue;
                }

                if (curPart is 0)
                {
                    if (char.IsWhiteSpace(c) || curPos == endPos)
                        continue;
                    else if (curPos == lastEndPos)
                    {
                        ListPool<IResult>.Shared.Return(results);
                        ListPool<IResult>.Shared.Return(paramResults);

                        StringBuilderPool.Shared.Return(builder);

                        return ResultUtils.Error("There must be at least one character of whitespace between arguments.");
                    }
                    else
                    {
                        if (arg == null)
                            arg = command.Arguments.Length > results.Count ? command.Arguments[results.Count] : null;

                        if (arg != null && arg.Options.IsRemainder)
                        {
                            builder.Append(c);
                            continue;
                        }

                        if (IsOpenQuote(c))
                        {
                            curPart = 1;
                            match = GetMatch(c);
                            continue;
                        }

                        curPart = 2;
                    }
                }

                string argString = null;

                if (curPart is 2)
                {
                    if (curPos == endPos || char.IsWhiteSpace(c))
                    {
                        argString = builder.ToString();
                        lastEndPos = curPos;
                    }
                    else
                        builder.Append(c);
                }
                else if (curPart is 1)
                {
                    if (c == match)
                    {
                        argString = builder.ToString();
                        lastEndPos = curPos + 1;
                    }
                    else
                        builder.Append(c);
                }

                if (argString != null)
                {
                    if (arg is null)
                    {
                        if (command.IgnoreExtraArguments)
                            break;
                        else
                        {
                            ListPool<IResult>.Shared.Return(results);
                            ListPool<IResult>.Shared.Return(paramResults);

                            StringBuilderPool.Shared.Return(builder);

                            return ResultUtils.Error("The input text has too many parameters.");
                        }
                    }

                    var parserResult = arg.Parser.Parse(ctx, argString);

                    if (!parserResult.IsSuccess)
                    {
                        ListPool<IResult>.Shared.Return(results);
                        ListPool<IResult>.Shared.Return(paramResults);

                        StringBuilderPool.Shared.Return(builder);

                        return ResultUtils.Error($"Failed to parse parameter at position {arg.Position}: '{arg.Name}' (error: '{parserResult.ReadErrorMessage()}')");
                    }

                    if (arg.Options.IsCatchAll)
                    {
                        paramResults.Add(parserResult);
                        curPart = 0;
                    }
                    else
                    {
                        results.Add(parserResult);

                        arg = null;
                        curPart = 0;
                    }

                    builder.Clear();
                }
            }

            if (arg != null && arg.Options.IsRemainder)
            {
                var parserResult = arg.Parser.Parse(ctx, builder.ToString());

                if (!parserResult.IsSuccess)
                {
                    ListPool<IResult>.Shared.Return(results);
                    ListPool<IResult>.Shared.Return(paramResults);

                    StringBuilderPool.Shared.Return(builder);

                    return ResultUtils.Error($"Failed to parse parameter at position {arg.Position}: '{arg.Name}' (error: '{parserResult.ReadErrorMessage()}')");
                }

                results.Add(parserResult);
            }

            if (isEscaping)
            {
                ListPool<IResult>.Shared.Return(results);
                ListPool<IResult>.Shared.Return(paramResults);

                StringBuilderPool.Shared.Return(builder);

                return ResultUtils.Error("Input text may not end on an incomplete escape.");
            }

            if (curPart is 2)
            {
                ListPool<IResult>.Shared.Return(results);
                ListPool<IResult>.Shared.Return(paramResults);

                StringBuilderPool.Shared.Return(builder);

                return ResultUtils.Error("A quoted parameter is incomplete.");
            }

            for (int i = results.Count; i < command.Arguments.Length; i++)
            {
                arg = command.Arguments[i];

                if (arg.Options.IsContext)
                {
                    results.Add(ResultUtils.Success(ctx));
                    continue;
                }

                if (arg.Options.IsSender)
                {
                    results.Add(ResultUtils.Success(ctx.Sender));
                    continue;
                }

                if (arg.Options.IsCast)
                {
                    results.Add(Cast(default, default, arg.CastOptions));
                    continue;
                }

                if (arg.Options.IsCatchAll)
                    continue;

                if (!arg.Options.IsOptional)
                {
                    ListPool<IResult>.Shared.Return(results);
                    ListPool<IResult>.Shared.Return(paramResults);

                    StringBuilderPool.Shared.Return(builder);

                    return ResultUtils.Error("The input text has too few parameters.");
                }

                results.Add(ResultUtils.Success(arg.Default));
            }

            StringBuilderPool.Shared.Return(builder);

            return ResultUtils.Success(new Tuple<IResult[], IResult[]>(ListPool<IResult>.Shared.ToArrayReturn(results), ListPool<IResult>.Shared.ToArrayReturn(paramResults)));
        }

        public static IResult Cast(Vector3 position, Vector3 forward, CommandArgumentCastOptions commandArgumentCastOptions)
        {
            var ray = new Ray(position, forward);
            var isHit = Physics.Raycast(ray, out var hit, commandArgumentCastOptions.Distance, commandArgumentCastOptions.Mask, QueryTriggerInteraction.Ignore);

            if (isHit)
                return ResultUtils.Success(hit.collider);
            else
                return ResultUtils.Error();
        }

        private static T[] ConvertParamsList<T>(IEnumerable<object> paramsList)
            => paramsList.Cast<T>().ToArray();
    }
}