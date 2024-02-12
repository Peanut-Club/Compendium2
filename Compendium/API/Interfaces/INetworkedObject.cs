using Compendium.API.Networking;

using UnityEngine;

namespace Compendium.API.Interfaces
{
    public interface INetworkedObject : IWorldObject<Vector3, Quaternion>
    {
        Vector3 Position { set; }
        Quaternion Rotation { set; }

        bool IsSpawned { get; set; }

        uint Id { get; }

        Identity Identity { get; }

        void Spawn();

        void UnSpawn();

        void Destroy();
    }
}