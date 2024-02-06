using PlayerRoles.FirstPersonControl.Spawnpoints;

using UnityEngine;

using System;

namespace Compendium.API.Roles.Spawnpoints
{
    public class SpawnPoint 
    {
        public static ISpawnPoint Get(Vector3 position, ISpawnpointHandler spawnpointHandler)
        {
            if (spawnpointHandler is RoomRoleSpawnpoint roomRoleSpawnpoint)
                return new RoomSpawnPoint(position, roomRoleSpawnpoint);
            else if (spawnpointHandler is BoundsRoleSpawnpoint boundsRoleSpawnpoint)
                return new BoundSpawnPoint(position, boundsRoleSpawnpoint);
            else
                throw new Exception($"Unrecognized spawnpoint handler: {spawnpointHandler.GetType().FullName}");
        }
    }
}