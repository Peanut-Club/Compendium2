using Common.Logging;

using System;
using System.Linq;

namespace Compendium.API.Utilities
{
    public static class CustomInfoUtils
    {
        public static LogOutput Log { get; } = new LogOutput("Custom Info Utils").Setup();

        public static bool IsValid(string customInfo)
        {
            if (customInfo.Contains('<'))
            {
                foreach (var token in customInfo.Split('<'))
                {
                    if (token.StartsWith("/", StringComparison.Ordinal) ||
                        token.StartsWith("b>", StringComparison.Ordinal) ||
                        token.StartsWith("i>", StringComparison.Ordinal) ||
                        token.StartsWith("size=", StringComparison.Ordinal) ||
                        token.Length is 0)
                        continue;

                    if (token.StartsWith("color=", StringComparison.Ordinal))
                    {
                        if (token.Length < 14 || token[13] != '>')
                        {
                            Log.Error($"Custom info has been REJECTED. \nreason: (Bad text reject) \ntoken: {token} \nInfo: {customInfo}");
                            return false;
                        }
                        else if (!Misc.AllowedColors.ContainsValue(token.Substring(6, 7)))
                        {
                            Log.Error($"Custom info has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {customInfo}");
                            return false;
                        }
                    }
                    else if (token.StartsWith("#", StringComparison.Ordinal))
                    {
                        if (token.Length < 8 || token[7] != '>')
                        {
                            Log.Error($"Custom info has been REJECTED. \nreason: (Bad text reject) \ntoken: {token} \nInfo: {customInfo}");
                            return false;
                        }
                        else if (!Misc.AllowedColors.ContainsValue(token.Substring(0, 7)))
                        {
                            Log.Error($"Custom info has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {customInfo}");
                            return false;
                        }
                    }
                    else
                    {
                        Log.Error($"Custom info has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {customInfo}");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}