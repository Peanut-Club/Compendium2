using Compendium.API.Enums;
using Compendium.API.Interfaces;
using Compendium.API.Roles.Scp079;
using Compendium.API.Utilities;

using PlayerRoles.PlayableScps.Scp079.Cameras;

using System.Linq;

using UnityEngine;

namespace Compendium.API
{
    public class Camera : Wrapper<Scp079Camera>, IWorldObject<Vector2>
    {
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

        }

        public static Camera Get(ushort camId)
        {

        }
    }
}