using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp049.Abilities
{
    public class Scp049CallAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp049.Scp049CallAbility>
    {
        public Scp049CallAbility(Player player, PlayerRoles.PlayableScps.Scp049.Scp049CallAbility scp049CallAbility) : base(player, scp049CallAbility)
        {
            CooldownTime = PlayerRoles.PlayableScps.Scp049.Scp049CallAbility.BaseCooldown;
        }

        public float CooldownTime { get; set; }
    }
}