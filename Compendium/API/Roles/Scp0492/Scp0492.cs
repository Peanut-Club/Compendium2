using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp0492.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492
{
    public class Scp0492 : SubroutinedRole
    {
        public Scp0492(ZombieRole zombieRole) : base(zombieRole)
        {
            Base = zombieRole;

            BloodlustAbility = GetRoutine<Scp0492BloodlustAbility>();
            ConsumeAbility = GetRoutine<Scp0492ConsumeAbility>();
            AttackAbility = GetRoutine<Scp0492AttackAbility>();
            AudioPlayer = GetRoutine<Scp0492AudioPlayer>();
        }

        public new ZombieRole Base { get; }

        public Scp0492BloodlustAbility BloodlustAbility { get; }
        public Scp0492ConsumeAbility ConsumeAbility { get; }
        public Scp0492AttackAbility AttackAbility { get; }
        public Scp0492AudioPlayer AudioPlayer { get; }
    }
}