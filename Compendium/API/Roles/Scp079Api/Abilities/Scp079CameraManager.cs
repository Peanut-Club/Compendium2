using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079CameraManager : AbilityWrapper<Scp079CurrentCameraSync>
    {
        public Scp079CameraManager(Player player, Scp079CurrentCameraSync ability) : base(player, ability) { }

        public Camera Camera
        {
            get => Camera.Get(Base.CurrentCamera);
            set
            {
                value ??= Camera.Default;
                value.IsActive = true;

                Base._camSet = true;
                Base._lastCam = value.Base;

                Base._requestedCamId = value.Id;
                Base._clientSwitchRequest = Scp079CurrentCameraSync.ClientSwitchState.SwitchingZone;
                Base._errorCode = Scp079HudTranslation.Zoom;

                Base.ServerSendRpc(true);

                Base._requestedCamId = 0;
                Base._clientSwitchRequest = Scp079CurrentCameraSync.ClientSwitchState.None;
            }
        }

        public Camera RequestedCamera
        {
            get => Camera.Get(RequestedCameraId);
        }

        public ushort CameraId
        {
            get => Camera.Id;
            set => Camera = Camera.Get(value);
        }

        public ushort RequestedCameraId
        {
            get => Base._requestedCamId;
        }

        public bool IsOnDefaultCamera
        {
            get => CameraId == Camera.DefaultId;
        }
    }
}