using Common.Values;

using PlayerRoles.FirstPersonControl.Spawnpoints;

using UnityEngine;

namespace Compendium.API.Roles.Spawnpoints
{
    public interface ISpawnPoint : IWrapper<ISpawnpointHandler>
    {
        Vector3 NextPosition { get; }
        Vector3 SpawnedPosition { get; }

        Vector3[] AllPositions { get; }

        float Angle { get; }
    }
}