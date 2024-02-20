using Common.Values;

namespace Compendium.API.Roles
{
    public class NoneRole : Role, IWrapper<PlayerRoles.NoneRole>
    {
        public NoneRole(PlayerRoles.NoneRole roleBase) : base(roleBase)
        {
            Base = roleBase;
        }

        public new PlayerRoles.NoneRole Base { get; }
    }
}