using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp106;

namespace Compendium.API.Roles.Scp106
{
    public class Scp106Sinkhole : AbilityWrapper<Scp106SinkholeController>
    {
        public Scp106Sinkhole(Player player, Scp106SinkholeController ability) : base(player, ability)
        {
            Cooldown = 20f;
        }

        public float Cooldown { get; set; }

        public float RemainingCooldown
        {
            get => Base.Cooldown.Remaining;
            set
            {
                Base.Cooldown.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public bool IsSubmerged
        {
            get => Base.State;
            set => Base.State = value;
        }
    }
}