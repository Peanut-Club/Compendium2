using PlayerRoles.PlayableScps.Scp106;

namespace Compendium.API.Roles.Abilities
{
    public class VigorAbility<TAbility> : AbilityWrapper<TAbility>
        where TAbility : Scp106VigorAbilityBase
    {
        public VigorAbility(Player player, TAbility ability) : base(player, ability) { }

        public float Vigor
        {
            get => Base.VigorAmount;
            set => Base.VigorAmount = value;
        }
    }
}