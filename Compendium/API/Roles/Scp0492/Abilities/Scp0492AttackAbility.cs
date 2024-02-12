using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492.Abilities
{
    public class Scp0492AttackAbility : AttackAbilityWrapper<ZombieRole>
    {
        public Scp0492AttackAbility(Player player, ZombieAttackAbility ability) : base(player, ability)
        {
            Base = ability;
        }

        public new ZombieAttackAbility Base { get; }
    }
}