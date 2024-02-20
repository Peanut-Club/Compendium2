using NorthwoodLib;

using System;

using Compendium.API.Enums;

namespace Compendium.API.Tokens
{
    public class BadgeToken : IToken
    {
        public DateTime Issued { get; }
        public DateTime Expires { get; }

        public TokenUsage Usage { get; }

        public PlayerPermissions Permissions { get; }

        public string UserId { get; }
        public string Nick { get; }

        public string Color { get; }
        public string Text { get; }

        public string Serial { get; }
        public string Server { get; }

        public string DecodedValue { get; }
        public string EncodedValue { get; }

        public string Signature { get; }

        public int Version { get; }
        public int Type { get; }

        public bool IsTest { get; }
        public bool IsStaff { get; }
        public bool IsManagement { get; }

        public bool CanGlobalBan { get; }
        public bool CanAccessRemoteAdmin { get; }
        public bool CanAccessOverwatch { get; }

        public BadgeToken(SignedToken signedToken, global::BadgeToken token)
        {
            if (signedToken is null)
                throw new ArgumentNullException(nameof(signedToken));

            if (token is null)
                throw new ArgumentNullException(nameof(token));

            Usage = TokenUsage.Authorization;

            Issued = token.IssuanceTime.Date;
            Expires = token.ExpirationTime.Date;

            UserId = token.UserId;
            Nick = token.Nickname;
            Serial = token.Serial;
            Server = token.IssuedBy;

            Version = token.TokenVersion;

            IsTest = token.TestSignature;

            Color = token.BadgeColor;
            Text = token.BadgeText;
            Type = token.BadgeType;

            CanGlobalBan = token.GlobalBanning;
            CanAccessRemoteAdmin = token.RemoteAdmin;
            CanAccessOverwatch = token.OverwatchMode;

            IsManagement = token.Management;
            IsStaff = token.Staff;

            Permissions = (PlayerPermissions)token.RaPermissions;

            EncodedValue = signedToken.token;
            Signature = signedToken.signature;

            DecodedValue = StringUtils.Base64Decode(EncodedValue);
        }
    }
}