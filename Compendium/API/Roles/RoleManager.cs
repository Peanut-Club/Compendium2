using Compendium.Patches.Functions.Roles;

using PlayerRoles;

namespace Compendium.API.Roles
{
    public class RoleManager
    {
        static RoleManager()
        {
            UpdateRolePatch.OnRoleChanged += OnRoleChanged;
        }

        private Role curRole;

        public RoleManager(Player player)
        {
            Owner = player;
            curRole = RoleUtils.ToAPIRole(player.Hub.roleManager.CurrentRole);
        }

        public Player Owner { get; }

        public Role Role
        {
            get => curRole;
            set => Set(value);
        }

        public RoleTypeId Id
        {
            get => curRole.Id;
            set => Set(value);
        }

        public RoleChangeReason SpawnReason
        {
            get => curRole.SpawnReason;
        }

        public RoleSpawnFlags SpawnFlags
        {
            get => curRole.SpawnFlags;
        }

        public void Set(Role role)
            => Set(role.Id, role.SpawnFlags, role.SpawnReason);

        public void Set(RoleTypeId role, RoleSpawnFlags spawnFlags = RoleSpawnFlags.All, RoleChangeReason changeReason = RoleChangeReason.RemoteAdmin)
            => RoleUtils.SetVanillaRole<PlayerRoleBase>(role, Owner.Hub, spawnFlags, changeReason);

        private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
        {
            var player = Player.Get(hub);

            if (player is null)
                return;

            player.Role.curRole = RoleUtils.ToAPIRole(newRole);
        }
    }
}