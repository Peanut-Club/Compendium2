using Common.Values;

using PlayerRoles;

using System;

using UnityEngine;

namespace Compendium.API.Roles
{
    public class Role : IWrapper<PlayerRoleBase>
    {
        public Role(Player player, PlayerRoleBase role)
        {
            Owner = player;
            Base = role;

            ChangedAt = DateTime.Now;
        }

        public string Name => Base.RoleName;

        public bool CanTriggerTesla => Base is ITeslaControllerRole controllerRole && controllerRole.CanActivateShock;
        public bool UsesUnitNames => Base is HumanRole humanRole && humanRole.UsesUnitNames;

        public byte UnitId => Base is HumanRole humanRole ? humanRole.UnitNameId : (byte)0;

        public int Power
        {
            get
            {
                if (Id is RoleTypeId.NtfPrivate)
                    return 1;
                else if (Id is RoleTypeId.NtfSpecialist)
                    return 2;
                else if (Id is RoleTypeId.NtfSergeant)
                    return 3;
                else
                    return 0;
            }
        }

        public Player Owner { get; }
        public PlayerRoleBase Base { get; }

        public Team Team => Base.Team;
        public RoleTypeId Id => Base.RoleTypeId;
        public Faction Faction => Base.Team.GetFaction(); 
        public RoleSpawnFlags SpawnFlags => Base.ServerSpawnFlags;
        public RoleChangeReason SpawnReason => Base.ServerSpawnReason;

        public DateTime ChangedAt { get; }
        public Color Color => Base.RoleColor;
        public TimeSpan ActiveTime => new TimeSpan(Base._activeTime.ElapsedTicks);

        public int GetArmorEfficacy(HitboxType hitboxType)
        {
            if (Base is not IArmoredRole armoredRole)
                return 0;

            return armoredRole.GetArmorEfficacy(hitboxType);
        }

        public bool CanDisarm(Player target)
            => Base is HumanRole humanRole && humanRole.AllowDisarming(target.Hub);

        public bool CanUndisarm(Player target)
            => Base is HumanRole humanRole && humanRole.AllowUndisarming(target.Hub);
    }
}