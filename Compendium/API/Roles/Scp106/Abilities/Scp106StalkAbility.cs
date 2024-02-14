using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp106.Abilities
{
    public class Scp106StalkAbility : VigorAbility<PlayerRoles.PlayableScps.Scp106.Scp106StalkAbility>
    {
        public Scp106StalkAbility(Player player, PlayerRoles.PlayableScps.Scp106.Scp106StalkAbility ability) : base(player, ability)
        {
        }

        public bool IsStalking
        {
            get => Base.IsActive;
            set => Base.IsActive = value;  
        }
    }
}