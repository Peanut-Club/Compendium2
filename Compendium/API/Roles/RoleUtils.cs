using Common.Logging;
using Common.Extensions;

using CustomPlayerEffects;

using GameObjectPools;

using PlayerRoles;

using System;
using System.Reflection;

using UnityEngine;

using CentralAuth;

namespace Compendium.API.Roles
{
    public static class RoleUtils
    {
        public static readonly LogOutput Log = new LogOutput("Role Utilities");
        public static readonly EventInfo OnRoleChangedEvent = typeof(PlayerRoleManager).Event("OnRoleChanged");

        public static Role ToAPIRole(PlayerRoleBase playerRole)
        {

        }

        public static void UpdateClientRole(ReferenceHub client)
        {
            foreach (var hub in ReferenceHub.AllHubs)
            {
                if (hub.Mode != ClientInstanceMode.ReadyClient)
                    continue;

                var roleId = client.roleManager.CurrentRole.RoleTypeId;

                if (client.roleManager.CurrentRole is IObfuscatedRole obfuscatedRole)
                {
                    roleId = obfuscatedRole.GetRoleForUser(hub);

                    if (client.roleManager.PreviouslySentRole.TryGetValue(hub.netId, out var sentRole)
                        && sentRole == roleId)
                        continue;
                }

                hub.connectionToClient.Send(new RoleSyncInfo(client, roleId, hub));

                client.roleManager.PreviouslySentRole[hub.netId] = roleId;
            }
        }

        public static TRole SetVanillaRole<TRole>(RoleTypeId roleId, ReferenceHub owner, RoleSpawnFlags spawnFlags, RoleChangeReason changeReason) where TRole : PlayerRoleBase
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(roleId, out var plyRole))
                throw new Exception($"Failed to get role template for role '{roleId}'");

            if (!PoolManager.Singleton.TryGetPoolObject(plyRole.gameObject, out var pooledRole, false) || pooledRole is not TRole role)
                throw new Exception($"Failed to get role template for role '{roleId}' (pool/cast failure)");

            if (owner != null)
            {
                var curRole = owner.roleManager.CurrentRole;

                if (owner.roleManager.CurrentRole != null && owner.roleManager._anySet)
                    owner.roleManager.CurrentRole.DisableRole(roleId);

                role.transform.parent = owner.transform;

                role.transform.localPosition = Vector3.zero;
                role.transform.localRotation = Quaternion.identity;

                owner.roleManager.CurrentRole = role;

                try
                {
                    role.Init(owner, changeReason, spawnFlags);
                }
                catch (Exception ex)
                {
                    Log.Error($"Role '{roleId}' ({role.GetType().Name}) failed to initialize for player '{owner.nicknameSync.MyNick}':\n{ex}");
                }

                try
                {
                    role.SetupPoolObject();
                }
                catch (Exception ex)
                {
                    Log.Error($"Role '{roleId}' ({role.GetType().Name}) failed to setup pool object for player '{owner.nicknameSync.MyNick}':\n{ex}");
                }

                SpawnProtected.TryGiveProtection(owner);

                UpdateClientRole(owner);

                OnRoleChangedEvent.Raise(null, owner, curRole, role);
            }

            return role;
        }
    }
}
