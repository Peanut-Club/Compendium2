using PlayerRoles.FirstPersonControl.Spawnpoints;

using UnityEngine;

namespace Compendium.API.Roles.Spawnpoints
{
    public class BoundSpawnPoint : ISpawnPoint
    {
        public BoundSpawnPoint(Vector3 pos, BoundsRoleSpawnpoint boundsRoleSpawnpoint)
        {
            Base = boundsRoleSpawnpoint;
            MinAngle = boundsRoleSpawnpoint._rotMin;
            MaxAngle = boundsRoleSpawnpoint._rotMax;
            AllPositions = boundsRoleSpawnpoint._positions;

            SpawnedPosition = pos;
        }

        public Vector3 NextPosition
        {
            get
            {
                if (Base.TryGetSpawnpoint(out var spawnPos, out _))
                    return spawnPos;

                return Vector3.zero;
            }
        }

        public Vector3 SpawnedPosition { get; }
        public Vector3[] AllPositions { get; }

        public float Angle
        {
            get => Random.Range(MinAngle, MaxAngle);
        }

        public float MaxAngle { get; }
        public float MinAngle { get; }

        public ISpawnpointHandler Base { get; }
    }
}