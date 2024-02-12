using Compendium.API.Roles.Abilities;

using Mirror;

using UnityEngine;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079TeslaAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp079.Scp079TeslaAbility>
    {
        public Scp079TeslaAbility(Player player, PlayerRoles.PlayableScps.Scp079.Scp079TeslaAbility ability) : base(player, ability) { }

        public float Cooldown
        {
            get => Base._cooldown;
            set => Base._cooldown = value;
        }

        public float RemainingCooldown
        {
            get => Mathf.Clamp((float)(Base._nextUseTime - NetworkTime.time), 0f, (float)NetworkTime.time);
            set => Base._nextUseTime = Mathf.Clamp((float)(NetworkTime.time - value), 0f, (float)NetworkTime.time);
        }

        public int ExpCost
        {
            get => Base._cost;
            set => Base._cost = value;
        }
    }
}