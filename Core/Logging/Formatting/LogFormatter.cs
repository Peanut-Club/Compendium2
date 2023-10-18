using System.Collections.Generic;

namespace Compendium.Logging.Formatting
{
    public static class LogFormatter
    {
        public static readonly Dictionary<LogFormatTag, char> Tags = new Dictionary<LogFormatTag, char>()
        {
            [LogFormatTag.Black] = '0',

            [LogFormatTag.DarkRed] = '1',
            [LogFormatTag.DarkGreen] = '2',
            [LogFormatTag.DarkYellow] = '3',
            [LogFormatTag.DarkBlue] = '4',
            [LogFormatTag.DarkMagenta] = '5',
            [LogFormatTag.DarkCyan] = '6',
            [LogFormatTag.DarkGray] = '8',

            [LogFormatTag.LightGray] = '7',

            [LogFormatTag.Red] = '9',
            [LogFormatTag.Green] = '!',
            [LogFormatTag.Orange] = '?',
            [LogFormatTag.Blue] = ':',
            [LogFormatTag.Magenta] = '-',
            [LogFormatTag.Cyan] = '_',
            [LogFormatTag.White] = '/',

            [LogFormatTag.Reset] = 'r',

            [LogFormatTag.BoldOn] = 'b',
            [LogFormatTag.BoldOff] = 'B',

            [LogFormatTag.ItalicOn] = 'o',
            [LogFormatTag.ItalicOff] = 'O',

            [LogFormatTag.UnderlineOn] = 'n',
            [LogFormatTag.UnderlineOff] = 'N',

            [LogFormatTag.StrikethroughOn] = 'm',
            [LogFormatTag.StrikethroughOff] = 'M'
        };

        public static string Wrap(this string message, LogFormatTag tag, bool addEndingTags = true)
        {
            switch (tag)
            {
                case LogFormatTag.BoldOn:
                    return addEndingTags ? $"&b{message}&B" : $"&b{message}";

                case LogFormatTag.ItalicOn:
                    return addEndingTags ? $"&o{message}&O" : $"&o{message}";

                case LogFormatTag.UnderlineOn:
                    return addEndingTags ? $"&n{message}&N" : $"&n{message}";

                case LogFormatTag.StrikethroughOn:
                    return addEndingTags ? $"&m{message}&M" : $"&m{message}";

                case LogFormatTag.ItalicOff:
                    return $"{message}&O";

                case LogFormatTag.StrikethroughOff:
                    return $"{message}&M";

                case LogFormatTag.BoldOff:
                    return $"{message}&B";

                case LogFormatTag.Reset:
                    return $"{message}&r";

                default:
                    return $"&{Tags[tag]}{message}&r";
            }
        }

        public static string Format(this string message)
        {
            var isPrefix = false;
            var escapeChar = (char)27;
            var newText = string.Empty;

            for (int x = 0; x < message.Length; x++)
            {
                if (message[x] == '&' && !isPrefix)
                {
                    isPrefix = true;
                    continue;
                }
                else if (isPrefix)
                {
                    switch (message[x])
                    {
                        case '0':
                            newText += $"{escapeChar}[30m";
                            break;

                        case '1':
                            newText += $"{escapeChar}[31m";
                            break;

                        case '2':
                            newText += $"{escapeChar}[32m";
                            break;

                        case '3':
                            newText += $"{escapeChar}[33m";
                            break;

                        case '4':
                            newText += $"{escapeChar}[34m";
                            break;

                        case '5':
                            newText += $"{escapeChar}[35m";
                            break;

                        case '6':
                            newText += $"{escapeChar}[36m";
                            break;

                        case '7':
                            newText += $"{escapeChar}[37m";
                            break;

                        case '8':
                            newText += $"{escapeChar}[90m";
                            break;

                        case '9':
                            newText += $"{escapeChar}[91m";
                            break;

                        case '!':
                            newText += $"{escapeChar}[92m";
                            break;

                        case '?':
                            newText += $"{escapeChar}[93m";
                            break;

                        case ':':
                            newText += $"{escapeChar}[94m";
                            break;

                        case '-':
                            newText += $"{escapeChar}[95m";
                            break;

                        case '_':
                            newText += $"{escapeChar}[96m";
                            break;

                        case '/':
                            newText += $"{escapeChar}[97m";
                            break;

                        case 'r':
                            newText += $"{escapeChar}[0m";
                            break;

                        case 'b':
                            newText += $"{escapeChar}[1m";
                            break;

                        case 'B':
                            newText += $"{escapeChar}[22m";
                            break;

                        case 'o':
                            newText += $"{escapeChar}[3m";
                            break;

                        case 'O':
                            newText += $"{escapeChar}[23m";
                            break;

                        case 'n':
                            newText += $"{escapeChar}[4m";
                            break;

                        case 'N':
                            newText += $"{escapeChar}[24m";
                            break;

                        case 'm':
                            newText += $"{escapeChar}[9m";
                            break;

                        case 'M':
                            newText += $"{escapeChar}[29m";
                            break;
                    }

                    isPrefix = false;
                    continue;
                }

                newText += message[x];
            }

            return newText;
        }
    }
}
