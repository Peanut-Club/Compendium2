using Compendium.Logging;
using Compendium.Pooling.Pools;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;

namespace Compendium.Configuration
{
    public static class ConfigurationReader
    {
        public static Dictionary<string, string> Read(string[] buffer, Func<string, bool> keyValidator)
        {
            var dict = DictionaryPool<string, string>.Shared.Rent();

            var isReading = false;
            var builder = StringBuilderPool.Shared.Rent();
            var key = string.Empty;

            for (int i = 0; i < buffer.Length; i++)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(buffer[i]))
                    {
                        if (isReading)
                            builder.AppendLine(buffer[i]);

                        continue;
                    }

                    if (buffer[i].StartsWith("#"))
                    {
                        if (isReading)
                            builder.AppendLine(buffer[i]);

                        continue;
                    }

                    var index = buffer[i].IndexOf(':');

                    if (index <= 0)
                    {
                        if (isReading)
                            builder.AppendLine(buffer[i]);

                        continue;
                    }

                    key = buffer[i].Substring(0, index);

                    if (string.IsNullOrWhiteSpace(key) || !keyValidator.SafeCall(key))
                    {
                        if (isReading)
                            builder.AppendLine(buffer[i]);

                        continue;
                    }

                    if (isReading && !string.IsNullOrWhiteSpace(key))
                    {
                        dict[key] = builder.ToString();
                        builder.Clear();
                    }

                    isReading = true;

                    builder.AppendLine(buffer[i].Substring(index, buffer[i].Length - index));
                }
                catch (Exception ex)
                {
                    Log.Critical("Configuration Reader", $"Failed to read buffer at index '{i}' ({buffer[i]})", ex);
                }
            }

            StringBuilderPool.Shared.Return(builder);

            return dict;
        }
    }
}