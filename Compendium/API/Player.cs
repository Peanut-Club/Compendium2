using CentralAuth;

using Compendium.API.Data;
using Compendium.API.Effects;
using Compendium.API.Extensions;
using Compendium.API.Inventory;
using Compendium.API.Roles;
using Compendium.API.Tokens;
using Compendium.API.Units;

using LiteNetLib;

using Mirror;
using Mirror.LiteNetLib4Mirror;

using PlayerRoles.FirstPersonControl;

using System.Collections.Generic;
using System.Net;

using UnityEngine;

namespace Compendium.API
{
    public class Player
    {
        private static readonly HashSet<Player> playerSet = new HashSet<Player>();

        public static IReadOnlyCollection<Player> List => playerSet;

        public static Player Get(ReferenceHub hub)
        {
            foreach (var ply in playerSet)
            {
                if (ply.Hub.netId == hub.netId)
                    return ply;
            }

            return null;
        }

        private UserId cachedId;

        internal Unit ntfUnit;

        public Player(ReferenceHub hub)
        {
            Hub = hub;

            Connection = hub.connectionToClient;
            Identity = hub.netIdentity;

            Peer = LiteNetLib4MirrorServer.Peers[ConnectionId];

            EndPoint = Peer.EndPoint;

            Ip = EndPoint.Address.ToString();
            Port = EndPoint.Port;
            FullIp = EndPoint.ToString();

            cachedId = UserId.Get(Hub.authManager.UserId);

            Roles = new RoleManager(this);
            Data = new PlayerDataManager(this);
            Effects = new EffectManager(this);
            Inventory = new InventoryManager(this);

            if (hub.authManager.AuthenticationResponse.SignedAuthToken != null
                && hub.authManager.AuthenticationResponse.AuthToken != null)
                AuthToken = new AuthToken(hub.authManager.AuthenticationResponse.SignedAuthToken, hub.authManager.AuthenticationResponse.AuthToken);

            if (hub.authManager.AuthenticationResponse.SignedBadgeToken != null
                && hub.authManager.AuthenticationResponse.BadgeToken != null)
                BadgeToken = new Tokens.BadgeToken(hub.authManager.AuthenticationResponse.SignedBadgeToken, hub.authManager.AuthenticationResponse.BadgeToken);

            playerSet.Add(this);
        }

        public ReferenceHub Hub { get; }

        public NetworkConnectionToClient Connection { get; }
        public NetworkIdentity Identity { get; }
        public NetPeer Peer { get; }

        public IPEndPoint EndPoint { get; }

        public InventoryManager Inventory { get; }
        public PlayerDataManager Data { get; }
        public EffectManager Effects { get; }
        public RoleManager Roles { get; }

        public AuthToken AuthToken { get; }
        public Tokens.BadgeToken BadgeToken { get; }

        public UserId UserId
        {
            get => cachedId;
            set
            {
                if (value is null || string.IsNullOrWhiteSpace(value.Value))
                    return;

                Hub.authManager.UserId = value.Value;
                cachedId = UserId.Get(Hub.authManager.UserId);
            }
        }

        public Unit Unit
        {
            get => ntfUnit;
            set => UnitManager.Assign(this, value);
        }

        public string Nick
        {
            get => Hub.nicknameSync.Network_myNickSync;
            set => Hub.nicknameSync.Network_myNickSync = value;
        }

        public string DisplayedNick
        {
            get => Hub.nicknameSync.Network_displayName;
            set => Hub.nicknameSync.Network_displayName = value;
        }

        public string DisplayedInfo
        {
            get => Hub.nicknameSync.Network_customPlayerInfoString;
            set => Hub.nicknameSync.Network_customPlayerInfoString = value;
        }

        public string SyncedId
        {
            get => Hub.authManager.NetworkSyncedUserId;
            set => Hub.authManager.NetworkSyncedUserId = value;
        }

        public string PrivateId
        {
            get => Hub.authManager._privUserId;
            set => Hub.authManager._privUserId = value;
        }

        public string Ip { get; }
        public string FullIp { get; }

        public int ConnectionId
        {
            get => Connection.connectionId;
        }

        public int Latency
        {
            get => Peer.Ping;
        }

        public int PlayerId
        {
            get => Hub.Network_playerId.Value;
            set => Hub.Network_playerId = new RecyclablePlayerId(value);
        }

        public int Port { get; }

        public uint NetworkId
        {
            get => Identity.netId;
            set => Identity.netId = value;
        }

        public uint RaPasswordAttempts
        {
            get => Hub.authManager._passwordAttempts;
            set => Hub.authManager._passwordAttempts = value;
        }

        public double RoundTripTime
        {
            get => Connection.rtt;
        }

        public bool ShowInPlayerList { get; set; } = true;
        public bool ShowInRemoteAdminList { get; set; } = true;
        public bool ShowInSpectatorList { get; set; } = true;

        public float RaPasswordCooldown
        {
            get => Hub.authManager._passwordCooldown;
            set => Hub.authManager._passwordCooldown = value;
        }

        public Vector3 Position
        {
            get => Hub.GetRealPosition();
            set => Hub.TryOverridePosition(Position, Rotation.eulerAngles);
        }

        public Quaternion Rotation
        {
            get => Hub.GetRealRotation();
            set => Hub.TryOverridePosition(Position, value.eulerAngles);
        }

        public ClientInstanceMode Mode
        {
            get => Hub.authManager.InstanceMode;
            set => Hub.authManager.InstanceMode = value;
        }

        public ClientInstanceMode? ForcedMode { get; set; }
    }
}