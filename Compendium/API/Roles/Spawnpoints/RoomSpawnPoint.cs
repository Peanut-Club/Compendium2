using Common.Pooling.Pools;

using MapGeneration;

using PlayerRoles.FirstPersonControl.Spawnpoints;

using System.Linq;

using UnityEngine;

namespace Compendium.API.Roles.Spawnpoints
{
    public class RoomSpawnPoint : ISpawnPoint
    {
        public RoomSpawnPoint(Vector3 position, RoomRoleSpawnpoint roomRoleSpawnpoint)
        {
            Base = roomRoleSpawnpoint;
            SpawnedPosition = position;

            Name = roomRoleSpawnpoint._fName;
            Shape = roomRoleSpawnpoint._fShape;
            Zone = roomRoleSpawnpoint._fZone;
            Angle = roomRoleSpawnpoint._lookAngle;

            Rooms = RoomIdUtils.FindRooms(Name, Zone, Shape).ToArray();

            var posList = ListPool<Vector3>.Shared.Rent();

            foreach (var bounds in roomRoleSpawnpoint._spawnpoints)
                posList.AddRange(bounds._positions);

            AllPositions = ListPool<Vector3>.Shared.ToArrayReturn(posList);
        }

        public RoomIdentifier[] Rooms { get; }

        public ISpawnpointHandler Base { get; }

        public Vector3 NextPosition
        {
            get
            {
                if (Base.TryGetSpawnpoint(out var spawnPoint, out _))
                    return spawnPoint;

                return Vector3.zero;
            }
        }

        public Vector3 SpawnedPosition { get; }

        public Vector3[] AllPositions { get; }

        public RoomName Name { get; }
        public RoomShape Shape { get; }

        public FacilityZone Zone { get; }

        public float Angle { get; }
    }
}