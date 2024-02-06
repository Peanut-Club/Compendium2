using Common.Values;

using Compendium.API.Roles.Other;

using PlayerRoles.PlayableScps.Scp049;

namespace Compendium.API.Roles.Scp049
{
    public class Scp049 : SubroutinedRole, IWrapper<Scp049Role>
    {
        public Scp049(Scp049Role scp049Role) : base(scp049Role)
        {
            Base = scp049Role;

            ResurrectAbility = Subroutines.Get<Abilities.Scp049ResurrectAbility>();
            AttackAbility = Subroutines.Get<Abilities.Scp049AttackAbility>();
            SenseAbility = Subroutines.Get<Abilities.Scp049SenseAbility>();
            CallAbility = Subroutines.Get<Abilities.Scp049CallAbility>();
        }

        public new Scp049Role Base { get; }

        public Abilities.Scp049ResurrectAbility ResurrectAbility { get; }
        public Abilities.Scp049AttackAbility AttackAbility { get; }
        public Abilities.Scp049SenseAbility SenseAbility { get; }
        public Abilities.Scp049CallAbility CallAbility { get; }
    }
}