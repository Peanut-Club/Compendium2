﻿using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Subroutines;

namespace Compendium.API.Roles.Abilities
{
    public class AttackAbilityWrapper<TRole> : AbilityWrapper<ScpAttackAbilityBase<TRole>>
        where TRole : FpcStandardScp
    {
        public AttackAbilityWrapper(Player player, ScpAttackAbilityBase<TRole> ability) : base(player, ability)
        {
            AttackDelay = ability.AttackDelay;
            AttackCooldown = ability.BaseCooldown;
            AttackDamage = ability.DamageAmount;

            IsSelfRepeating = ability.SelfRepeating;
        }

        public float AttackDelay { get; set; }
        public float AttackCooldown { get; set; }
        public float AttackDamage { get; set; }
         
        public bool CanBeTriggered
        {
            get => Base.CanTriggerAbility;
        }

        public bool IsSelfRepeating { get; set; }
    }
}