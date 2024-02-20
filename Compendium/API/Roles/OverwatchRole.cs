using Common.Values;

namespace Compendium.API.Roles
{
    public class OverwatchRole : SpectatorRole, IWrapper<PlayerRoles.Spectating.OverwatchRole>
    {
        public OverwatchRole(PlayerRoles.Spectating.OverwatchRole roleBase) : base(roleBase)
        {
            Base = roleBase;
        }

        public new PlayerRoles.Spectating.OverwatchRole Base { get; }
    }
}