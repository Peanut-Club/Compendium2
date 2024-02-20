using Compendium.API.Roles.Abilities;

using MapGeneration;

using PlayerRoles.PlayableScps.Scp079;

using System.Linq;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079ScannerZoneFilter : AbilityWrapper<Scp079ScannerZoneSelector>
    {
        public Scp079ScannerZoneFilter(Player player, Scp079ScannerZoneSelector ability) : base(player, ability) { }

        public bool AnySelected
        {
            get => Base._selectedZones.Any(st => st);
        }

        public FacilityZone[] Selected
        {
            get => Base.SelectedZones;
        }

        public FacilityZone[] Available
        {
            get => Scp079ScannerZoneSelector.AllZones;
        }

        public bool IsSelected(FacilityZone zone)
            => Base.GetZoneStatus(zone);
    }
}