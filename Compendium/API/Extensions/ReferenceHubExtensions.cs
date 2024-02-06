using Common.Pooling.Pools;

using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp079;

using System.Collections.Generic;

using UnityEngine;

namespace Compendium.API.Extensions
{
    public static class ReferenceHubExtensions
    {
        public static Player[] GetPlayerArray(this IEnumerable<ReferenceHub> hubs)
        {
            var players = ListPool<Player>.Shared.Rent();

            foreach (var hub in hubs)
            {
                var player = Player.Get(hub);

                if (player != null)
                    players.Add(player);
            }

            return ListPool<Player>.Shared.ToArrayReturn(players);
        }

        public static Vector3 GetRealPosition(this ReferenceHub hub)
        {
            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is IFpcRole fpcRole
                && fpcRole.FpcModule != null)
                return fpcRole.FpcModule.Position;

            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is Scp079Role scp079Role
                && scp079Role.CurrentCamera != null)
                return scp079Role.CurrentCamera.CameraPosition;

            return hub.PlayerCameraReference.position;
        }

        public static Quaternion GetRealRotation(this ReferenceHub hub)
        {
            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is IFpcRole fpcRole
                && fpcRole.FpcModule != null && fpcRole.FpcModule.MouseLook != null)
                return fpcRole.FpcModule.MouseLook.TargetHubRotation;

            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is Scp079Role scp079Role
                && scp079Role.CurrentCamera != null)
                return new Quaternion(scp079Role.HorizontalRotation, scp079Role.RollRotation, scp079Role.VerticalRotation, 0f);

            return hub.PlayerCameraReference.rotation;
        }

        public static bool TryGetFpmm(this ReferenceHub hub, out FirstPersonMovementModule fpmm)
        {
            if (hub.roleManager.CurrentRole is null || hub.roleManager.CurrentRole is not IFpcRole fpcRole || fpcRole.FpcModule is null || !fpcRole.FpcModule.ModuleReady)
            {
                fpmm = null;
                return false;
            }

            fpmm = fpcRole.FpcModule;
            return true;
        }
    }
}
