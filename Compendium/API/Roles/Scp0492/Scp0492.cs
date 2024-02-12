using Common.Values;

using Compendium.API.Roles.Abilities;
using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp0492.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492
{
    public class Scp0492 : SubroutinedRole, IWrapper<ZombieRole>
    {
        public Scp0492(ZombieRole zombieRole) : base(zombieRole)
        {
            Base = zombieRole;

            ConsumeAbility = GetRoutine<AbilityWrapper<ZombieConsumeAbility>>();
            AttackAbility = GetRoutine<AbilityWrapper<ZombieAttackAbility>>();

            BloodlustAbility = GetRoutine<Scp0492BloodlustAbility>();
            AudioPlayer = GetRoutine<Scp0492AudioPlayer>();
        }

        public new ZombieRole Base { get; }

        public AbilityWrapper<ZombieConsumeAbility> ConsumeAbility { get; }
        public AbilityWrapper<ZombieAttackAbility> AttackAbility { get; }

        public Scp0492BloodlustAbility BloodlustAbility { get; }
        public Scp0492AudioPlayer AudioPlayer { get; }
    }
}