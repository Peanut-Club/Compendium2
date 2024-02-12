using Common.Utilities;

using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079
{
    public class Scp079Aux : AbilityWrapper<Scp079AuxManager>
    {
        internal float? customRegenSpeed;

        public Scp079Aux(Player player, Scp079AuxManager ability) : base(player, ability) { }

        public float Current
        {
            get => Base.CurrentAux;
            set => Base.CurrentAux = value;
        }

        public float Maximum
        {
            get => Base.MaxAux;
            set => Base._maxPerTier[Base._tierManager.AccessTierIndex] = value;
        }

        public float Regeneration
        {
            get => customRegenSpeed ?? Base.RegenSpeed;
            set => CodeUtils.InlinedElse(value < 0f, true, () => customRegenSpeed = null, () => customRegenSpeed = value, null, null);
        }

        public void Regenerate()
            => Base.Regenerate();

        public void RegenerateFull()
            => Current = Maximum;
    }
}
