using CentralAuth;

using Common.Extensions;
using Common.Values;

using System;
using System.Linq;

using Compendium.API.Enums;

namespace Compendium.API
{
    public class UserIdValue : IGetValue<string>
    {
        public const string ServerId = PlayerAuthenticationManager.DedicatedId;
        public const string HostId = PlayerAuthenticationManager.HostId;

        public const string NorthwoodId = "northwood";
        public const string DiscordId = "discord";
        public const string SteamId = "steam";

        public static readonly UserIdValue Null = new UserIdValue(string.Empty, string.Empty, 0, UserIdType.Unknown);

        public string Value { get; }
        public string Clear { get; }

        public long Parsed { get; }

        public UserIdType Type { get; }

        public UserIdValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            if (value == ServerId || value == HostId)
            {
                Value = value;
                Clear = value;

                Parsed = 0;

                Type = UserIdType.Server;
                return;
            }

            if (!value.TrySplit('@', true, 2, out var idParts))
                throw new FormatException($"A valid user ID must contain a single @ sign!");

            Value = value;
            Clear = idParts[0];

            if (long.TryParse(idParts[0], out var parsed))
                Parsed = parsed;

            switch (idParts[1].ToLower())
            {
                case NorthwoodId:
                    Type = UserIdType.Northwood;
                    break;

                case DiscordId:
                    Type = UserIdType.Discord;
                    break;

                case SteamId:
                    Type = UserIdType.Steam;
                    break;

                default:
                    throw new Exception($"Unrecognized user ID type: {idParts[1]}");
            }
        }

        public UserIdValue(string value, string clear, long parsed, UserIdType type)
        {
            Value = value;
            Clear = clear;
            Parsed = parsed;
            Type = type;
        }

        public static UserIdValue Parse(string value)
            => TryParse(value, out var uid) ? uid : Null;

        public static bool TryParse(string value, out UserIdValue id)
        {
            if (value == HostId || value == ServerId)
            {
                id = new UserIdValue($"{value}@server", value, 0, UserIdType.Server);
                return true;
            }    

            if (value.Count(c => c == '@') == 1)
            {
                id = new UserIdValue(value);
                return true;
            }

            var idLength = value.Length;

            if (idLength == 17)
            {
                long.TryParse(value, out var steamParsed);

                id = new UserIdValue($"{value}@steam", value, steamParsed, UserIdType.Steam);
                return true;
            }
            else if (idLength <= 19 && idLength > 17)
            {
                long.TryParse(value, out var discordParsed);

                id = new UserIdValue($"{value}@discord", value, discordParsed, UserIdType.Discord);
                return true;
            }

            id = null;
            return false;
        }
    }
}