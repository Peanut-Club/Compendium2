using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp079.Map;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079MapAbility : AbilityWrapper<Scp079MapToggler>
    {
        public Scp079MapAbility(Player player, Scp079MapToggler ability) : base(player, ability) { }

        public bool IsOpened
        {
            get => Base.SyncState;
        }
    }
}