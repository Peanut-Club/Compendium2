using Mirror;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079.Cameras;

using UnityEngine;

using Utils.NonAllocLINQ;

namespace Compendium.API.Utilities
{
    public static class CameraUtils
    {
        public static bool TrySetRotation(this Scp079Camera cam, Vector2 rotation)
        {
            cam.VerticalAxis.TargetValue = rotation.x;
            cam.HorizontalAxis.TargetValue = rotation.y;
            return cam.TrySyncCamera();
        }

        public static bool TrySyncCamera(this Scp079Camera camera)
        {
            var compHub = ReferenceHub.AllHubs.FirstOrDefault(hub => hub.roleManager.CurrentRole.RoleTypeId is RoleTypeId.Scp079, null);

            if (compHub is null)
                return false;

            var writer = NetworkWriterPool.Get();

            writer.WriteUShort(camera.SyncId);
            camera.WriteAxes(writer);

            return SubroutineUtils.ServerSendSync<Scp079CameraRotationSync>(compHub, compHub, writer, RoleTypeId.Scp079);
        }
    }
}