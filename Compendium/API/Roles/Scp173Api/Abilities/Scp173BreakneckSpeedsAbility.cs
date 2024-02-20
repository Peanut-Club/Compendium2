using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp173Api.Abilities
{
    public class Scp173BreakneckSpeedsAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp173.Scp173BreakneckSpeedsAbility>
    {
        public Scp173BreakneckSpeedsAbility(Player player, PlayerRoles.PlayableScps.Scp173.Scp173BreakneckSpeedsAbility ability) : base(player, ability) { }
    
        public bool IsActive
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        public float RemainingCooldown
        {
            get => Base.Cooldown.Remaining;
            set
            {
                Base.Cooldown.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public float RemainingDuration
        {
            get => Base._disableTime - Base.Elapsed;
            set => Base._disableTime = Base.Elapsed + value;
        }
    }
}