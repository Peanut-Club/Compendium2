using Common.Values;

using Compendium.API.Core;

using PlayerRoles;

namespace Compendium.API.Roles
{
    public class FakedRoleList : FakedList<RoleTypeId>
    {
        public override void OnUpdated(Player target, OptionalValue<RoleTypeId> previousValue, RoleTypeId currentValue)
            => target.Connection.Send(new RoleSyncInfo(Target.Base, currentValue, target.Base));

        public override void OnRemoved(Player target, RoleTypeId value)
            => target.Connection.Send(new RoleSyncInfo(Target.Base, Target.Base.GetRoleId(), target.Base));
    }
}