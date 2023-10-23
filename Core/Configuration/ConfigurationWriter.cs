using Compendium.Pooling.Pools;

using System;
using System.Collections.Generic;

namespace Compendium.Configuration
{
    public static class ConfigurationWriter
    {
        public static string Write(IEnumerable<ConfigurationTargetInfo> configurations)
        {
            var builder = StringBuilderPool.Shared.Rent($"# Configuration file generated at {DateTime.Now.ToString("F")}");

            foreach (var config in configurations)
            {
                builder.AppendLine();

                if (!string.IsNullOrWhiteSpace(config.Description))
                    builder.AppendLine($"# {config.Description}");

                builder.Append($"{config.Name}: ");

                var cfgValue = config.Value;

                if (cfgValue is null)
                {
                    builder.Append("null");
                    continue;
                }

                var parsedValue = ConfigurationSerializer.Serialize(config.Value);
                var parsedLines = parsedValue.Split('\n', '\r');

                if (parsedLines.Length >= 2)
                {
                    for (int i = 0; i < parsedLines.Length; i++)
                    {
                        if (i is 0)
                        {
                            builder.Append(parsedLines[i]);
                            continue;
                        }

                        builder.AppendLine(parsedLines[i]);
                    }
                }
                else
                {
                    builder.Append(parsedLines[0]);
                }
            }

            return StringBuilderPool.Shared.StringReturn(builder);
        }
    }
}