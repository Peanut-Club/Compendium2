﻿using Common.Values;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.PlayableScps.Scp173;
using System;

using UnityEngine;

namespace Compendium.API.Roles
{
    public class Role : IWrapper<PlayerRoleBase>
    {
        internal Player roleOwner;

        public Role(PlayerRoleBase roleBase)
        {
            roleBase.TryGetOwner(out roleBase._lastOwner);
            roleOwner = Player.Get(roleBase._lastOwner);

            Base = roleBase;

            FakedRole = new FakedRoleList();
            FakedRole.Target = Player;
        }

        public PlayerRoleBase Base { get; }
        public FakedRoleList FakedRole { get; }

        public Player Player
        {
            get => roleOwner;
        }

        public string Name
        {
            get => Base.RoleName;
        }

        public bool IsAlive
        {
            get => Type.IsAlive();
        }

        public bool IsDead
        {
            get => !IsAlive;
        }

        public bool IsScp
        {
            get => Type.IsAlive() && !Type.IsHuman();
        }

        public bool IsHuman
        {
            get => Type.IsAlive() && Type.IsHuman();
        }

        public bool IsPlayable
        {
            get => Type != RoleTypeId.None && Type != RoleTypeId.Tutorial;
        }

        public bool IsSpectating
        {
            get => Type is RoleTypeId.Spectator || Type is RoleTypeId.Overwatch;
        }

        public bool IsNtf
        {
            get => Type is RoleTypeId.NtfCaptain || Type is RoleTypeId.NtfPrivate || Type is RoleTypeId.NtfSergeant || Type is RoleTypeId.NtfSpecialist;
        }

        public bool IsChaos
        {
            get => Type is RoleTypeId.ChaosConscript || Type is RoleTypeId.ChaosMarauder || Type is RoleTypeId.ChaosRepressor || Type is RoleTypeId.ChaosRifleman;
        }

        public RoleTypeId Type
        {
            get => Base.RoleTypeId;
            set => Set(value, RoleSpawnFlags.None, RoleChangeReason.RemoteAdmin);
        }

        public Team Team
        {
            get => Type.GetTeam();
        }

        public Faction Faction
        {
            get => Team.GetFaction();
        }

        public Color Color
        {
            get => Base.RoleColor;
        }

        public TimeSpan Time
        {
            get => TimeSpan.FromSeconds(Base.ActiveTime);
        }

        public void Set(RoleTypeId role, RoleSpawnFlags spawnFlags = RoleSpawnFlags.All, RoleChangeReason changeReason = RoleChangeReason.RemoteAdmin)
            => Player.Base.roleManager.ServerSetRole(role, changeReason, spawnFlags);

        public static Role Create(PlayerRoleBase playerRoleBase)
            => Create<Role>(playerRoleBase);

        public static TRole Create<TRole>(PlayerRoleBase playerRoleBase) where TRole : Role
        {
            Role role = null;

            switch (playerRoleBase)
            {
                case Scp049Role scp049Role:
                    role = new Scp049.Scp049(scp049Role);
                    break;

                case ZombieRole zombieRole:
                    role = new Scp0492.Scp0492(zombieRole);
                    break;

                case Scp079Role scp079Role:
                    role = new Scp079.Scp079(scp079Role);
                    break;

                case Scp096Role scp096Role:
                    role = new Scp096.Scp096(scp096Role);
                    break;

                case Scp106Role scp106Role:
                    role = new Scp106.Scp106(scp106Role);
                    break;

                case Scp173Role scp173Role:
                    role = new Scp173.Scp173(scp173Role);
                    break;
            }

            if (role != null)
                return (TRole)role;

            return default;
        }
    }
}