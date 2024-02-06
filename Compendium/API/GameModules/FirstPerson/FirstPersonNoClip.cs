using Common.Utilities;
using Common.Values;

using PlayerRoles.FirstPersonControl;

namespace Compendium.API.GameModules.FirstPerson
{
    public class FirstPersonNoClip : IWrapper<FpcNoclip>
    {
        public FirstPersonNoClip(FpcNoclip fpcNoclip, FirstPersonModule firstPersonModule)
        {
            Base = fpcNoclip;
            Module = firstPersonModule;
        }

        public FpcNoclip Base { get; }
        public FirstPersonModule Module { get; }

        public bool IsPermitted
        {
            get => FpcNoclip.IsPermitted(Base._hub);
            set => CodeUtils.InlinedElse(value, value == IsPermitted, () => FpcNoclip.PermitPlayer(Base._hub), () => FpcNoclip.UnpermitPlayer(Base._hub), null, null);
        }

        public bool IsActive
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        public bool WasActive
        {
            get => Base._wasEnabled;
        }
    }
}