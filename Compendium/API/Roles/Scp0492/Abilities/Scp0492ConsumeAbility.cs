using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492.Abilities
{
    public class Scp0492ConsumeAbility : RagdollAbilityWrapper<ZombieRole>
    {
        public Scp0492ConsumeAbility(Player player, ZombieConsumeAbility ability) : base(player, ability)
        {
            Base = ability;
        }

        public new ZombieConsumeAbility Base { get; }
    }
}