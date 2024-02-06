using Common.Values;
using Common.Utilities;
using Common.Extensions;

using Compendium.API.UserId;
using Compendium.API.Utilities;
using Compendium.API.Modules;

using Compendium.API.Roles;
using Compendium.API.Roles.Interfaces;

using Compendium.API.GameModules.FirstPerson;
using Compendium.API.GameModules.Subroutines;
using Compendium.API.GameModules.Stats;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;

using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerRoles.RoleAssign;

using UnityEngine;

using PluginAPI.Core;

using InventorySystem.Disarming;

using Mirror;

using Utils.Networking;

using LiteNetLib;

using Mirror.LiteNetLib4Mirror;

using CustomPlayerEffects;
using CentralAuth;

namespace Compendium.API
{
    public class Player : ModuleManager, IWrapper<ReferenceHub>
    {
        private static readonly List<Player> players = new List<Player>();

        public static IReadOnlyList<Player> List
        {
            get => players;
        }

        public static Player Host { get; private set; }
        public static Player Server { get; private set; }

        public static Player Get(uint networkId)
            => players.FirstOrDefault(p => p.NetworkId == networkId);

        public static Player Get(int playerId)
            => players.FirstOrDefault(p => p.PlayerId == playerId);

        public static Player GetConnection(int connectionId)
            => players.FirstOrDefault(p => p.ConnectionId == connectionId);

        public static Player Get(ReferenceHub hub)
            => players.FirstOrDefault(p => p.Base == hub);

        public Player(GameObject hubGo)
            : this(ReferenceHub.GetHub(hubGo))
        { }

        public Player(ReferenceHub hub)
        {
            if (hub is null)
                throw new ArgumentNullException(nameof(hub));

            Base = hub;
            GameObject = hub.gameObject;

            UserId = new UserIdValue(Base.authManager.UserId);

            Peer = LiteNetLib4MirrorServer.Peers[ConnectionId];

            if (Base.authManager.AuthenticationResponse.AuthToken != null)
                AuthToken = new Tokens.AuthToken(Base.authManager.AuthenticationResponse.SignedAuthToken, Base.authManager.AuthenticationResponse.AuthToken);

            if (Base.authManager.AuthenticationResponse.BadgeToken != null)
                BadgeToken = new Tokens.BadgeToken(Base.authManager.AuthenticationResponse.SignedBadgeToken, Base.authManager.AuthenticationResponse.BadgeToken);

            Stats = Add<StatManager>();
            Subroutines = Add<SubroutineManager>();

            players.Add(this);
        }

        public ReferenceHub Base { get; }
        public UserIdValue UserId { get; }
        public GameObject GameObject { get; }
        public NetPeer Peer { get; }

        public Tokens.AuthToken AuthToken { get; }
        public Tokens.BadgeToken BadgeToken { get; }

        public FirstPersonModule FirstPerson { get; private set; }
        public SubroutineManager Subroutines { get; private set; }
        public StatManager Stats { get; private set; }
        public Role Role { get; private set; }

        public IFirstPersonRole FirstPersonRole { get; private set; }
        public IPositionalRole PositionalRole { get; private set; }
        public ISubroutineRole SubroutineRole { get; private set; }
        public ICameraRole CameraRole { get; private set; }

        public Transform Camera
        {
            get => Base.PlayerCameraReference;
        }

        public IPEndPoint IpAddress
        {
            get => Peer.EndPoint;
        }

        public NetworkIdentity Identity
        {
            get => Base.netIdentity;
        }

        public NetworkConnectionToClient Connection
        {
            get => Base.connectionToClient;
        }

        public Player Cuffer
        {
            get
            {
                var disarmedEntries = DisarmedPlayers.Entries;

                for (int i = 0; i < disarmedEntries.Count; i++)
                {
                    if (disarmedEntries[i].DisarmedPlayer != NetworkId)
                        continue;

                    return Get(disarmedEntries[i].Disarmer);
                }

                return null;
            }
            set
            {
                var disarmedEntries = DisarmedPlayers.Entries;

                for (int i = 0; i < disarmedEntries.Count; i++)
                {
                    if (disarmedEntries[i].DisarmedPlayer == NetworkId)
                    {
                        disarmedEntries.RemoveAt(i);
                        break;
                    }
                }

                if (value != null)
                {
                    DisarmedPlayers.Entries.Add(new DisarmedPlayers.DisarmedEntry(NetworkId, value.NetworkId));

                    new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                }
            }
        }

        public Player SpectatedPlayer
        {
            get => List.FirstOrDefault(p => p.Base.IsSpectatedBy(Base));
        }

        public Player[] Spectators
        {
            get => List.WhereArray(p => Base.IsSpectatedBy(p.Base));
        }

        public ScpSpawnPreferences.SpawnPreferences ScpPreferences
        {
            get => ScpSpawnPreferences.Preferences.TryGetValue(ConnectionId, out var preferences) ? preferences : default;
            set => ScpSpawnPreferences.Preferences[ConnectionId] = value;
        }

        public Vector3 Position
        {
            get => Role is IPositionalRole positionalRole ? positionalRole.Position : Camera.position;
            set => CodeUtils.InlinedIf(Role is IPositionalRole, true, () => ((IPositionalRole)Role).Position = value, null);
        }

        public Quaternion Rotation
        {
            get => Role is ICameraRole cameraRole ? cameraRole.Rotation : Camera.rotation;
            set => CodeUtils.InlinedIf(Role is ICameraRole, true, () => ((ICameraRole)Role).Rotation = value, null);
        }

        public PlayerInfoArea InfoArea
        {
            get => Base.nicknameSync.Network_playerInfoToShow;
            set => Base.nicknameSync.Network_playerInfoToShow = value;
        }

        public PlayerPermissions Permissions
        {
            get => (PlayerPermissions)Base.serverRoles.Permissions;
            set => Base.serverRoles.Permissions = (ulong)value;
        }

        public ClientInstanceMode InstanceMode
        {
            get => Base.authManager.InstanceMode;
            set => Base.authManager.InstanceMode = value;
        }
   
        public ClientInstanceMode? ForcedInstanceMode { get; set; }

        public byte KickPower
        {
            get => Base.serverRoles.KickPower;
            set => Base.serverRoles.Group!.KickPower = value;
        }

        public int PlayerId
        {
            get => Base.Network_playerId.Value;
            set => Base.Network_playerId = new RecyclablePlayerId(value);
        }

        public int ConnectionId
        {
            get => Connection.connectionId;
        }

        public int Latency
        {
            get => Peer.Ping;
        }

        public uint NetworkId
        {
            get => Identity.netId;
        }

        public float InfoViewRange
        {
            get => Base.nicknameSync.NetworkViewRange;
            set => Base.nicknameSync.NetworkViewRange = value;
        }

        public string PrivateUserId
        {
            get => Base.authManager._privUserId;
            set => Base.authManager._privUserId = value;
        }

        public string SyncedUserId
        {
            get => Base.authManager.NetworkSyncedUserId;
            set => Base.authManager.NetworkSyncedUserId = value;
        }

        public string RankColor
        {
            get => Base.serverRoles.Network_myColor;
            set => Base.serverRoles.Network_myColor = value;
        }

        public string RankName
        {
            get => Base.serverRoles.Network_myText;
            set => Base.serverRoles.Network_myText = value;
        }

        public string Name
        {
            get => Base.nicknameSync.Network_myNickSync;
            set => Base.nicknameSync.Network_myNickSync = value;
        }

        public string DisplayName
        {
            get => Base.nicknameSync.Network_displayName;
            set => Base.nicknameSync.Network_displayName = value;
        }

        public string CustomInfo
        {
            get => Base.nicknameSync.Network_customPlayerInfoString;
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                if (string.IsNullOrWhiteSpace(value))
                {
                    RemoveInfoArea(PlayerInfoArea.CustomInfo);

                    Base.nicknameSync.Network_customPlayerInfoString = string.Empty;

                    return;
                }

                if (!CustomInfoUtils.IsValid(value))
                    return;

                AddInfoArea(PlayerInfoArea.CustomInfo);

                Base.nicknameSync.Network_customPlayerInfoString = value;
            }
        }

        public bool IsCuffed
        {
            get => DisarmedPlayers.Entries.Any(entry => entry.DisarmedPlayer == NetworkId);
            set => CodeUtils.InlinedElse(value, true, () => Base.inventory.SetDisarmedStatus(ReferenceHub.LocalHub.inventory), () => Base.inventory.SetDisarmedStatus(null), null, null);
        }

        public bool IsHost
        {
            get => Base.isLocalPlayer;
        }

        public bool IsReady
        {
            get => InstanceMode is ClientInstanceMode.ReadyClient && GameObject != null;
        }

        public bool IsOnline
        {
            get => GameObject != null;
        }

        public bool IsNorthwoodStaff
        {
            get => Base.authManager.NorthwoodStaff;
        }

        public bool IsNorthwoodModerator
        {
            get => Base.authManager.RemoteAdminGlobalAccess;
        }

        public bool HasCustomName
        {
            get => Base.nicknameSync.HasCustomName;
        }

        public bool HasDoNotTrack
        {
            get => Base.authManager.DoNotTrack;
        }

        public bool HasStaffChatAccess
        {
            get => Base.serverRoles.AdminChatPerms;
            set => Base.serverRoles.AdminChatPerms = value;
        }

        public bool HasRemoteAdminAccess
        {
            get => Base.serverRoles.RemoteAdmin;
            set => Base.serverRoles.RemoteAdmin = value;
        }

        public bool HasOverwatchEnabled
        {
            get => Base.serverRoles.IsInOverwatch;
            set => Base.serverRoles.IsInOverwatch = value;
        }

        public bool HasBypassModeEnabled
        {
            get => Base.serverRoles.BypassMode;
            set => Base.serverRoles.BypassMode = value;
        }

        public bool HasGodModeEnabled
        {
            get => Base.characterClassManager.GodMode;
            set => Base.characterClassManager.GodMode = value;
        }

        public bool HasNoClipEnabled
        {
            get => Stats.AdminFlags.IsNoClip;
            set => Stats.AdminFlags.IsNoClip = value;
        }

        public bool HasNoClipPermission
        {
            get => FpcNoclip.IsPermitted(Base);
            set => CodeUtils.InlinedElse(value, value == HasNoClipPermission, () => FpcNoclip.PermitPlayer(Base), () => FpcNoclip.UnpermitPlayer(Base), null, null);
        }

        public bool HasHiddenBadge
        {
            get => !string.IsNullOrWhiteSpace(Base.serverRoles.HiddenBadge);
            set => CodeUtils.InlinedElse(value, value == HasHiddenBadge, () => Base.serverRoles.TryHideTag(), () => Base.serverRoles.RefreshLocalTag(), null, null);
        }

        public bool HasSpawnProtection
        {
            get => Base.playerEffectsController.GetEffect<SpawnProtected>().IsEnabled;
            set => CodeUtils.InlinedElse(value, value == HasSpawnProtection, () => Base.playerEffectsController.EnableEffect<SpawnProtected>(SpawnProtected.SpawnDuration), () => Base.playerEffectsController.DisableEffect<SpawnProtected>(), null, null);
        }

        public bool HasReservedSlot
        {
            get => ReservedSlot.HasReservedSlot(UserId.Value, out _);
            set => CodeUtils.InlinedElse(value, value == HasReservedSlot, () => ReservedSlots.Add(UserId.Value), () => ReservedSlots.Remove(UserId.Value), ReservedSlots.Reload, ReservedSlots.Reload);
        }

        public bool HasWhitelist
        {
            get => WhiteList.IsWhitelisted(UserId.Value);
            set => CodeUtils.InlinedElse(value, value == HasWhitelist, () => Whitelist.Add(UserId.Value), () => Whitelist.Remove(UserId.Value), WhiteList.Reload, WhiteList.Reload);
        }

        public bool HasInfoArea(PlayerInfoArea infoArea)
            => (InfoArea & infoArea) != 0;

        public void RemoveInfoArea(PlayerInfoArea infoArea)
        {
            if (!HasInfoArea(infoArea))
                return;

            InfoArea &= ~infoArea;
        }

        public void AddInfoArea(PlayerInfoArea infoArea)
        {
            if (HasInfoArea(infoArea))
                return;

            InfoArea |= infoArea;
        }

        public bool IsSpectatedBy(Player target)
            => target != null && target != this && Base.IsSpectatedBy(target.Base);

        public bool IsSpectating(Player target)
            => target != null && target != this && target.Base.IsSpectatedBy(Base);

        public void OpenRemoteAdmin()
            => Base.serverRoles.OpenRemoteAdmin();

        public void CloseRemoteAdmin()
            => Base.serverRoles.TargetSetRemoteAdmin(false);

        internal void UpdateRole()
        {
            Role = Role.Create(Base.roleManager.CurrentRole);

            if (Role is IFirstPersonRole firstPersonRole)
            {
                FirstPersonRole = firstPersonRole;
                FirstPerson = firstPersonRole.Module;
            }
            else
            {
                FirstPersonRole = null;
                FirstPerson = null;
            }

            if (Role is ISubroutineRole subroutineRole)
                SubroutineRole = subroutineRole;
            else
                SubroutineRole = null;

            if (Role is IPositionalRole positionalRole)
                PositionalRole = positionalRole;
            else
                PositionalRole = null;

            if (Role is ICameraRole cameraRole)
                CameraRole = cameraRole;
            else
                CameraRole = null;
        }
    }
}
