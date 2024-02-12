using Compendium.API.Roles.Abilities;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079ScannerTeamFilter : AbilityWrapper<Scp079ScannerTeamFilterSelector>
    {
        public Scp079ScannerTeamFilter(Player player, Scp079ScannerTeamFilterSelector ability) : base(player, ability) { }

        public bool AnySelected
        {
            get => Base.AnySelected;
        }

        public Team[] Selected
        {
            get => Base.SelectedTeams;
        }

        public Team[] Available
        {
            get => Base._availableFilters;
        }

        public bool IsSelected(Team team)
            => Base.GetTeamStatus(team);
    }
}