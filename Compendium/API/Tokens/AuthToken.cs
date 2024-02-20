using NorthwoodLib;

using System;

using Compendium.API.Enums;

namespace Compendium.API.Tokens
{
    public class AuthToken : IToken
    {
        public DateTime Issued { get; }
        public DateTime Expires { get; }

        public TokenUsage Usage { get; }

        public string Ip { get; }
        public string UserId { get; }
        public string Nick { get; }

        public string Serial { get; }

        public string Server { get; }

        public string Vac { get; }

        public string PublicKey { get; }
        public string Challenge { get; }

        public string DecodedValue { get; }
        public string EncodedValue { get; }

        public string Signature { get; }

        public int Version { get; }
        public int Asn { get; }

        public bool IsTest { get; }
        public bool IsBanned { get; }
        public bool IsDoNotTrack { get; }
        public bool IsBetaOwner { get; }

        public bool CanBypassBans { get; }
        public bool CanBypassGeoBlock { get; }
        public bool CanBypassWhitelist { get; }

        public bool ShouldSkipIpCheck { get; }

        public bool HasGlobalBadge { get; }

        public AuthToken(SignedToken signedToken, AuthenticationToken token)
        {
            if (signedToken is null)
                throw new ArgumentNullException(nameof(signedToken));

            if (token is null)
                throw new ArgumentNullException(nameof(token));

            Usage = TokenUsage.Authorization;

            Issued = token.IssuanceTime.Date;
            Expires = token.ExpirationTime.Date;

            Ip = token.RequestIp;
            UserId = token.UserId;
            Nick = token.Nickname;
            Serial = token.Serial;
            Server = token.IssuedBy;
            Vac = token.VacSession;

            PublicKey = token.PublicKey;
            Challenge = token.Challenge;

            Version = token.TokenVersion;
            Asn = token.Asn;

            IsTest = token.TestSignature;
            IsDoNotTrack = token.DoNotTrack;
            IsBetaOwner = token.PrivateBetaOwnership;
            IsBanned = token.GlobalBan == "NO";

            CanBypassBans = token.BypassBans;
            CanBypassGeoBlock = token.BypassGeoRestrictions;
            CanBypassWhitelist = token.BypassWhitelists;

            ShouldSkipIpCheck = token.SkipIpCheck;

            HasGlobalBadge = token.GlobalBadge;

            EncodedValue = signedToken.token;
            Signature = signedToken.signature;

            DecodedValue = StringUtils.Base64Decode(EncodedValue);
        }
    }
}
