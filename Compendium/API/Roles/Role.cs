using Common.Values;

using Compendium.API.Roles.Faking;

using PlayerRoles;

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

            OnInitialized();
        }

        public PlayerRoleBase Base { get; }

        public FakedRoleList FakedRole { get; }

        public Player Player
        {
            get => roleOwner;
            set => Base._lastOwner = (roleOwner = FakedRole.Target = value).Base;
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

        internal virtual void OnInitialized() { }

        public static Role Create(PlayerRoleBase playerRoleBase)
            => Create<Role>(playerRoleBase);

        public static TRole Create<TRole>(PlayerRoleBase playerRoleBase) where TRole : Role
        {

        }
    }
}