using Common.Values;

using Compendium.API.Enums;
using Compendium.API.Roles.Abilities;
using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp0492Api.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492Api
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

        public Scp0492AudioType SyncedAudio
        {
            get => AudioPlayer.SyncedAudio;
            set => AudioPlayer.SyncedAudio = value;
        }

        public bool IsLookingAtBloodlustTarget
        {
            get => BloodlustAbility.IsLookingAtTarget;
            set => BloodlustAbility.IsLookingAtTarget = value;
        }

        public bool HasAnyBloodlustTargets
        {
            get => BloodlustAbility.HasAnyTargets;
        }

        public void PlayGrowl()
            => AudioPlayer.Growl();

        public void PlayAngryGrowl()
            => AudioPlayer.AngryGrowl();

        public void PlayAudio(Scp0492AudioType audioType)
            => AudioPlayer.Play(audioType);
    }
}