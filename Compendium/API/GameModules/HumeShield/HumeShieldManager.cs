using Common.Values;

using PlayerRoles.PlayableScps.HumeShield;

namespace Compendium.API.GameModules.HumeShield
{
    public class HumeShieldManager : IWrapper<HumeShieldModuleBase>
    {
        public HumeShieldManager(HumeShieldModuleBase humeShieldModuleBase)
        {
            Base = humeShieldModuleBase;

            Maximum = Base.HsMax;
            Regeneration = Base.HsRegeneration;
        }

        public HumeShieldModuleBase Base { get; }

        public float Current
        {
            get => Base.HsCurrent;
            set => Base.HsCurrent = value;
        }

        public float Maximum { get; set; }
        public float Regeneration { get; set; }
    }
}