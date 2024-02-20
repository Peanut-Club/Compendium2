using Compendium.API.Roles.Abilities;

using Interactables.Interobjects;

using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079ElevatorUseAbility : AbilityWrapper<Scp079ElevatorStateChanger>
    {
        public Scp079ElevatorUseAbility(Player player, Scp079ElevatorStateChanger ability) : base(player, ability) { }

        public int Cost
        {
            get => Base._cost;
            set => Base._cost = value;
        }

        public ElevatorDoor Elevator
        {
            get => Base._lastElevator;
        }
    }
}