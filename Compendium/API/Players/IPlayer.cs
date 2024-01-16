using CentralAuth;

using Common.Values;

using Compendium.API.Inventory;
using Compendium.API.Nicknames;
using Compendium.API.Roles;
using Compendium.API.Tokens;

using LiteNetLib;

using Mirror;

using System.Net;
using UnityEngine;

namespace Compendium.API.Players
{
    public interface IPlayer : IWrapper<ReferenceHub>
    {
        NetPeer Peer { get; }

        NetworkIdentity Identity { get; }
        NetworkConnection Connection { get; }

        IPEndPoint Ip { get; }

        UserIdValue Id { get; }

        PlayerModifiers Modifiers { get; }

        INicknameManager Name { get; }
        IInventoryManager Inventory { get; }
        IRoleManager Role { get; }
        IPlayerPosition Position { get; }

        IToken AuthToken { get; }
        IToken BadgeToken { get; }

        ClientInstanceMode Mode { get; }

        Vector3 Size { get; set; }
        Vector3 ModelSize { get; }

        int ConnectionId { get; }
        int PlayerId { get; set; }

        int Latency { get; }

        uint NetworkId { get; }
        uint PasswordAttempts { get; set; }

        double RoundTripTime { get; }

        float PasswordCooldown { get; set; }

        float Scale { get; set; }
        float ModelScale { get; }
    }
}