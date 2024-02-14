using Common.Values;

using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp106.Abilities;

namespace Compendium.API.Roles.Scp106
{
    public class Scp106 : SubroutinedRole, IWrapper<PlayerRoles.PlayableScps.Scp106.Scp106Role>
    {
        public Scp106(PlayerRoles.PlayableScps.Scp106.Scp106Role scpRole) : base(scpRole)
        {
            Base = scpRole;
        }

        public new PlayerRoles.PlayableScps.Scp106.Scp106Role Base { get; }

        public Scp106HuntersAtlasAbility HuntersAtlasAbility { get; }
        public Scp106AttackAbility AttackAbility { get; }
        public Scp106StalkAbility StalkAbility { get; }
        public Scp106Sinkhole Sinkhole { get; }

        public bool IsSubmerged
        {
            get => StalkAbility.IsStalking;
            set => StalkAbility.IsStalking = value;
        }
    }
}