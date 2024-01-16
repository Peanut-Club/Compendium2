using Compendium.API.Doors;
using Compendium.API.Extensions;
using Compendium.API.Rooms;
using Compendium.API.Utilities;
using Compendium.API.Waypoints;
using Compendium.API.Zones;

using PlayerRoles;
using PlayerRoles.FirstPersonControl;

using RelativePositioning;

using System;

using UnityEngine;

namespace Compendium.API.Players
{
    public class PlayerPositionManager : IPlayerPosition
    {
        public PlayerPositionManager(Player owner)
            => Owner = owner;

        public Vector3 Position
        {
            get => Owner.Base.GetRealPosition();
            set => Owner.Base.TryOverridePosition(value, Vector3.zero);
        }

        public Vector3 SpawnPosition
        {
            get => Owner.Role.Role.SpawnPosition;
        }

        public Vector3 VectorRotation
        {
            get => Owner.Base.PlayerCameraReference.eulerAngles;
            set => ClientRotation = new ClientRotation(value);
        }

        public ClientRotation ClientRotation
        {
            get => new ClientRotation(Owner.Base.transform.rotation);
            set
            {
                if (Owner.Base.TryGetFpmm(out var fpmm) && fpmm.MouseLook != null)
                    fpmm.MouseLook.ApplySyncValues(value.HorizontalAxis, value.VerticalAxis);
            }
        }

        public Quaternion Rotation
        {
            get => Owner.Base.GetRealRotation();
            set => ClientRotation = new ClientRotation(value);
        }

        public RelativePosition RelativePosition
        {
            get => new RelativePosition(Position);
            set => Position = value.Position;
        }

        public IPlayer Owner { get; }

        public IRoom Room { get; set; }
        public IZone Zone { get; set; }
        public IDoor Door { get; }
        public IWaypoint Waypoint { get; set; }

        public RoomType RoomType { get; set; }
        public ZoneType ZoneType { get; set; }
        public DoorType DoorType { get; }
        public WaypointType WaypointType { get; }

        public ushort HorizontalAxis
        {
            get => ClientRotation.HorizontalAxis;
            set => ClientRotation = new ClientRotation(value, VerticalAxis);
        }

        public ushort VerticalAxis
        {
            get => ClientRotation.VerticalAxis;
            set => ClientRotation = new ClientRotation(HorizontalAxis, value);
        }

        public bool IsInPocketDimension => ZoneType is ZoneType.PocketZone;
        public bool IsInEntranceZone => ZoneType is ZoneType.EntranceZone;
        public bool IsInSurfaceZone => ZoneType is ZoneType.SurfaceZone;
        public bool IsInHeavyZone => ZoneType is ZoneType.HeavyZone;
        public bool IsInLightZone => ZoneType is ZoneType.LightZone;

        public bool IsFrozen { get; set; }

        public bool IsGrounded
        {
            get => Owner.Base.TryGetFpmm(out var fpmm) && fpmm.IsGrounded;
        }

        public float DistanceToGround
        {
            get
            {
                if (IsGrounded || !Owner.Base.TryGetFpmm(out var fpmm))
                    return 0f;

                var rayPos = Owner.Base.GetPlayerCastPosition();
                rayPos.y -= fpmm.CharacterControllerSettings.Height;
                return RaycastUtils.CastDownwardsGround(rayPos, float.MaxValue).distance;
            }
        }

        public void Teleport(Vector3 targetPosition)
        {
            if (Owner.Base.TryGetFpmm(out var fpmm))
                fpmm.ServerOverridePosition(targetPosition, Vector3.zero);
        }

        public void Teleport(Vector3 targetPosition, Quaternion targetRotation)
        {
            if (Owner.Base.TryGetFpmm(out var fpmm))
                fpmm.ServerOverridePosition(targetPosition, targetRotation.ToClientRotationVector());
        }

        public void TeleportToPocketDimension()
        {
            throw new NotImplementedException();
        }

        public void SimulatePocketDimensionEscape()
            => SimulatePocketDimensionEscape(Array.Empty<RoomType>());

        public void SimulatePocketDimensionEscape(params ZoneType[] escapeZones)
        {
            throw new NotImplementedException();
        }

        public void SimulatePocketDimensionEscape(params RoomType[] escapeRooms)
        {
            throw new NotImplementedException();
        }

        public Vector3 TeleportToRandomPosition()
        {
            throw new NotImplementedException();
        }

        public Vector3 TeleportToRandomPosition(params Vector3[] possiblePositions)
        {
            var randomPos = possiblePositions.RandomItem();
            Teleport(randomPos);
            return randomPos;
        }

        public IRoom TeleportToRandomRoom()
            => TeleportToRandomRoom(Array.Empty<RoomType>());

        public IRoom TeleportToRandomRoom(params ZoneType[] zoneFilter)
        {
            throw new NotImplementedException();
        }

        public IRoom TeleportToRandomRoom(params RoomType[] roomFilter)
        {
            throw new NotImplementedException();
        }

        public Vector3 TeleportToRandomSpawnPosition()
            => TeleportToRandomSpawnPosition(Array.Empty<RoleTypeId>());

        public Vector3 TeleportToRandomSpawnPosition(params RoleTypeId[] possibleRoles)
        {
            throw new NotImplementedException();
        }

        public IZone TeleportToRandomZone()
            => TeleportToRandomZone(Array.Empty<ZoneType>());

        public IZone TeleportToRandomZone(params ZoneType[] zoneFilter)
        {
            throw new NotImplementedException();
        }
    }
}
