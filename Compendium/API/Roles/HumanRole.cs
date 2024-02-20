using Common.Values;

using Compendium.API.Roles.Other;

using Respawning.NamingRules;

namespace Compendium.API.Roles
{
    public class HumanRole : FirstPersonRole, IWrapper<PlayerRoles.HumanRole>
    {
        public HumanRole(PlayerRoles.HumanRole roleBase) : base(roleBase)
        {
            Base = roleBase;
        }

        public new PlayerRoles.HumanRole Base { get; }

        public bool IsUnitRole
        {
            get => Base.UsesUnitNames;
        }

        public byte UnitId
        {
            get => Base.UnitNameId;
        }

        public string UnitName
        {
            get => (IsUnitRole && UnitNameMessageHandler.ReceivedNames.TryGetValue(Base.AssignedSpawnableTeam, out var names) && UnitId < names.Count) ? names[UnitId] : null;
        }
    }
}