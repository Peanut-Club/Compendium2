using PlayerRoles;

using System;

namespace Compendium.Patches.Functions.Roles
{
    public static class UpdateRolePatch
    {
        public static event Action<ReferenceHub, PlayerRoleBase, PlayerRoleBase> OnRoleChanged;
    }
}