using PlayerRoles.PlayableScps.Scp079.Cameras;

using CentralAuth;

using PlayerRoles;

using Mirror;

using MEC;

namespace Compendium.API.Utilities
{
    public static class CameraUtils
    {
        public static void ServerSetRotation(this Scp079Camera cam, float rotation)
        {
            cam.HorizontalAxis.TargetValue = rotation;
            cam.ServerSyncCamera();
        }

        public static void ServerSyncCamera(this Scp079Camera camera)
        {
            foreach (var hub in ReferenceHub.AllHubs)
            {
                if (hub.Mode != ClientInstanceMode.ReadyClient)
                    continue;

                hub.ServerSendSync<Scp079CameraRotationSync>(ReferenceHub.HostHub, RoleTypeId.Scp079, writer =>
                {
                    writer.WriteUShort(camera.SyncId);
                    camera.WriteAxes(writer);
                });
            }
        }

        internal static void InitRound()
        {
            ReferenceHub.HostHub.roleManager.ServerSetRole(RoleTypeId.Scp079, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);

            Timing.CallDelayed(0.3f, () =>
            {
                foreach (var hub in ReferenceHub.AllHubs)
                {
                    if (hub.Mode != ClientInstanceMode.ReadyClient || hub.IsAlive())
                        continue;

                    hub.connectionToClient.Send(new RoleSyncInfo(ReferenceHub.HostHub, RoleTypeId.None, hub));
                }
            });
        }
    }
}