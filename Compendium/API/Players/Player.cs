using CentralAuth;

using Common.Extensions;

using Compendium.API.Inventory;
using Compendium.API.Nicknames;
using Compendium.API.Roles;
using Compendium.API.Tokens;

using LiteNetLib;

using Mirror;
using Mirror.LiteNetLib4Mirror;

using PlayerRoles.FirstPersonControl;

using System;
using System.Net;

using UnityEngine;

namespace Compendium.API.Players
{
    public class Player : IPlayer
    {
        public Player(ReferenceHub hub)
        {
            Base = hub;

            Identity = hub.netIdentity;
            NetworkId = hub.netIdentity.netId;

            Connection = hub.connectionToClient;
            ConnectionId = hub.connectionToClient.connectionId;

            Peer = LiteNetLib4MirrorServer.Peers[hub.connectionToClient.connectionId];

            Ip = Peer._remoteEndPoint;

            AuthToken = TrySetAuthToken();
            BadgeToken = TrySetBadgeToken();

            Id = new UserIdValue(hub.authManager.UserId);

            Modifiers = new PlayerModifiers();

            Name = new PlayerNameManager(this);
            Position = new PlayerPositionManager(this);
        }

        public ReferenceHub Base { get; }

        public NetPeer Peer { get; }

        public NetworkIdentity Identity { get; }
        public NetworkConnection Connection { get; }

        public IPEndPoint Ip { get; }

        public UserIdValue Id { get; }

        public PlayerModifiers Modifiers { get; }

        public Vector3 Size
        {
            get => Base.transform.localScale;
            set => Base.transform.localScale = value;
        }

        public Vector3 ModelSize
        {
            get
            {
                if (Base.roleManager.CurrentRole is not IFpcRole fpcRole)
                    return Vector3.zero;

                if (fpcRole.FpcModule is null || fpcRole.FpcModule.CharacterModelInstance is null)
                    return Vector3.zero;

                return fpcRole.FpcModule.CharacterControllerSettings.Center * fpcRole.FpcModule.CharacterControllerSettings.Radius;
            }
        }

        public INicknameManager Name { get; }
        public IInventoryManager Inventory { get; }
        public IRoleManager Role { get; }
        public IPlayerPosition Position { get; }

        public IToken AuthToken { get; }
        public IToken BadgeToken { get; }

        public ClientInstanceMode Mode
        {
            get => Base.authManager._targetInstanceMode;
            set => Base.authManager._targetInstanceMode = value;
        }

        public int ConnectionId { get; }

        public int PlayerId
        {
            get => Base.Network_playerId.Value;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (value < RecyclablePlayerId._autoIncrement)
                    value = RecyclablePlayerId._autoIncrement++;

                if (RecyclablePlayerId.FreeIds.Contains(value))
                    RecyclablePlayerId.FreeIds.Remove(value);

                Base.Network_playerId = new RecyclablePlayerId(value);
            }
        }

        public int Latency
        {
            get => Peer.Ping;
        }

        public uint NetworkId { get; }

        public uint PasswordAttempts
        {
            get => Base.authManager._passwordAttempts;
            set => Base.authManager._passwordAttempts = value;
        }

        public double RoundTripTime
        {
            get => Connection is NetworkConnectionToClient clientConn ? clientConn.rtt : Peer.RoundTripTime;
        }

        public float PasswordCooldown
        {
            get => Base.authManager._passwordCooldown;
            set => Base.authManager._passwordCooldown = value;
        }

        public float Scale
        {
            get => Size.magnitude;
            set => Size *= value;
        }

        public float ModelScale
        {
            get => ModelSize.magnitude;
        }

        internal AuthToken TrySetAuthToken()
        {
            if (Base.authManager.AuthenticationResponse.AuthToken != null)
                return new AuthToken(Base.authManager.AuthenticationResponse.SignedAuthToken, Base.authManager.AuthenticationResponse.AuthToken);

            return null;
        }

        internal Tokens.BadgeToken TrySetBadgeToken()
        {
            if (Base.authManager.AuthenticationResponse.BadgeToken != null)
                return new Tokens.BadgeToken(Base.authManager.AuthenticationResponse.SignedBadgeToken, Base.authManager.AuthenticationResponse.BadgeToken);

            return null;
        }
    }
}