using Compendium.API.Core;
using Compendium.API.Enums;
using Compendium.API.Interfaces;
using Compendium.API.Roles.Scp079Api;
using Compendium.API.Utilities;

using MapGeneration;

using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Compendium.API
{
    public class Camera : Wrapper<Scp079Camera>, IWorldObject<Vector2>
    {
        private static readonly Dictionary<Scp079Camera, Camera> cameras = new Dictionary<Scp079Camera, Camera>();
        private static readonly Dictionary<ushort, Camera> camerasById = new Dictionary<ushort, Camera>();

        public static Camera Default { get; private set; }

        public static ushort DefaultId
        {
            get => Default.Id;
        }

        public Camera(Scp079Camera baseValue) : base(baseValue) { }

        public WorldObjectType ObjectType { get; } = WorldObjectType.Camera;

        public Player User
        {
            get => Player.List.FirstOrDefault(p => p.Role is Scp079 scp && scp.CurrentCamera != null && scp.CurrentCamera == this);
        }

        public Vector3 Position
        {
            get => Base.CameraPosition;
        }

        public Vector2 Rotation
        {
            get => new Vector2(Base.VerticalRotation, Base.HorizontalRotation);
            set => Base.TrySetRotation(value);
        }

        public bool IsActive
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        public ushort Id
        {
            get => Base.SyncId;
        }

        public static Camera Get(Scp079Camera scp079Camera)
        {
            if (cameras.TryGetValue(scp079Camera, out var camera))
                return camera;

            return null;
        }

        public static Camera Get(ushort camId)
        {
            if (camerasById.TryGetValue(camId, out var camera))
                return camera;

            return null;
        }

        internal static void PrepCameras()
        {
            cameras.Clear();
            camerasById.Clear();

            Default = null;

            foreach (var interactable in Scp079InteractableBase.AllInstances)
            {
                if (interactable is not Scp079Camera camera)
                    continue;

                var apiCam = new Camera(camera);

                cameras[camera] = apiCam;
                camerasById[camera.SyncId] = apiCam;

                if (apiCam.Base.Room != null && apiCam.Base.Room.Name is RoomName.Hcz079 && Default is null)
                    Default = apiCam;
            }
        }
    }
}