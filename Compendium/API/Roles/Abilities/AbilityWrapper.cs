using Compendium.API.Core;
using Compendium.API.GameModules.Subroutines;

using PlayerRoles.Subroutines;

namespace Compendium.API.Roles.Abilities
{
    public class AbilityWrapper<TAbility> : PlayerWrapper<TAbility>, ISubroutine
        where TAbility : SubroutineBase
    {
        public AbilityWrapper(Player player, TAbility ability) : base(player, ability)
        {
            IsEnabled = true;
        }

        public bool IsEnabled { get; set; }
    }
}