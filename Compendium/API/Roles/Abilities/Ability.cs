using Common.Values;

using Compendium.API.GameModules.Subroutines;

using PlayerRoles.Subroutines;

namespace Compendium.API.Roles.Abilities
{
    public class Ability<TAbility> : IWrapper<TAbility>, ISubroutine
        where TAbility : SubroutineBase
    {
        public Ability(Player player, TAbility ability)
        {
            Player = player;
            Base = ability;
            IsEnabled = true;
        }

        public Player Player { get; }
        public TAbility Base { get; }

        public bool IsEnabled { get; set; }
    }
}