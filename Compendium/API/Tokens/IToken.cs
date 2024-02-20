using System;

using Compendium.API.Enums;

namespace Compendium.API.Tokens
{
    public interface IToken
    {
        DateTime Issued { get; }
        DateTime Expires { get; }

        TokenUsage Usage { get; }

        string UserId { get; }
        string Nick { get; }
        string Serial { get; }
        string Server { get; }

        string DecodedValue { get; }

        string EncodedValue { get; }
        string Signature { get; }

        int Version { get; }

        bool IsTest { get; }
    }
}