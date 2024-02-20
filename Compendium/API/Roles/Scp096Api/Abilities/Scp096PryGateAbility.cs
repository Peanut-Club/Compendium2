using Compendium.API.Roles.Abilities;

using Interactables.Interobjects;

using PlayerRoles.PlayableScps.Scp096;

namespace Compendium.API.Roles.Scp096Api.Abilities
{
    public class Scp096PryGateAbility : AbilityWrapper<Scp096PrygateAbility>
    {
        public Scp096PryGateAbility(Player player, Scp096PrygateAbility ability) : base(player, ability)
        {

        }

        public PryableDoor Door
        {
            get => Base._syncDoor;
            set
            {
                Base._syncDoor = value;
                Base.ServerSendRpc(true);
            }
        }
    }
}
