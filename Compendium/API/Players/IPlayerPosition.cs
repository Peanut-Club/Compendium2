using Compendium.API.Doors;
using Compendium.API.Rooms;
using Compendium.API.Utilities;
using Compendium.API.Waypoints;
using Compendium.API.Zones;

using PlayerRoles;

using RelativePositioning;

using UnityEngine;

namespace Compendium.API.Players
{
    public interface IPlayerPosition : IPlayerModule
    {
        Vector3 Position { get; set; }
        Vector3 SpawnPosition { get; }
        Vector3 VectorRotation { get; set; }

        Quaternion Rotation { get; set; }

        ClientRotation ClientRotation { get; set; }

        RelativePosition RelativePosition { get; set; }

        IRoom Room { get; set; }
        IZone Zone { get; set; }
        IDoor Door { get; }
        IWaypoint Waypoint { get; set; }

        RoomType RoomType { get; set; }
        ZoneType ZoneType { get; set; }
        DoorType DoorType { get; }
        WaypointType WaypointType { get; }

        ushort HorizontalAxis { get; set; }
        ushort VerticalAxis { get; set; }

        bool IsInPocketDimension { get; }
        bool IsInEntranceZone { get; }
        bool IsInSurfaceZone { get; }
        bool IsInHeavyZone { get; }
        bool IsInLightZone { get; }

        bool IsGrounded { get; }
        bool IsFrozen { get; set; }

        float DistanceToGround { get; }

        void Teleport(Vector3 targetPosition);
        void Teleport(Vector3 targetPosition, Quaternion targetRotation);

        void TeleportToPocketDimension();

        void SimulatePocketDimensionEscape();
        void SimulatePocketDimensionEscape(params ZoneType[] escapeZones);
        void SimulatePocketDimensionEscape(params RoomType[] escapeRooms);

        IRoom TeleportToRandomRoom();
        IRoom TeleportToRandomRoom(params ZoneType[] zoneFilter);
        IRoom TeleportToRandomRoom(params RoomType[] roomFilter);

        IZone TeleportToRandomZone();
        IZone TeleportToRandomZone(params ZoneType[] zoneFilter);

        Vector3 TeleportToRandomPosition();
        Vector3 TeleportToRandomPosition(params Vector3[] possiblePositions);

        Vector3 TeleportToRandomSpawnPosition();
        Vector3 TeleportToRandomSpawnPosition(params RoleTypeId[] possibleRoles);
    }
}