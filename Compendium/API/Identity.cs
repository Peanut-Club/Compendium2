using Common.Utilities;

using Compendium.API.Core;

using Mirror;

using System.Collections.Generic;

using UnityEngine;

namespace Compendium.API
{
    public class Identity : Wrapper<NetworkIdentity>
    {
        private static readonly Dictionary<uint, Identity> identities = new Dictionary<uint, Identity>();

        public Identity(NetworkIdentity baseValue) : base(baseValue) { }

        public uint Asset
        {
            get => Base.assetId;
            set => Base.assetId = value;
        }

        public uint Id
        {
            get => Base.netId;
            set => Base.netId = value;
        }

        public ulong Scene
        {
            get => Base.sceneId;
            set => Base.sceneId = value;
        }

        public bool Authority
        {
            get => Base.isOwned;
            set => Base.isOwned = value;
        }

        public bool IsClient
        {
            get => Base.isClient;
        }

        public bool IsServer
        {
            get => Base.isServer;
        }

        public bool IsInstantiated
        {
            get => Base.SpawnedFromInstantiate;
        }

        public bool IsDestroyed
        {
            get => Base.destroyCalled;
        }

        public bool IsSpawned
        {
            get => NetworkServer.spawned.ContainsKey(Id);
            set => CodeUtils.InlinedElse(value, true, Spawn, UnSpawn, null, null);
        }

        public GameObject GameObject
        {
            get => Base.gameObject;
        }

        public Transform Transform
        {
            get => Base.transform;
        }

        public NetworkConnectionToClient Client
        {
            get => Base.connectionToClient;
        }

        public NetworkConnection Server
        {
            get => Base.connectionToServer;
        }

        public NetworkBehaviour[] Behaviours
        {
            get => Base.NetworkBehaviours;
            set
            {
                if (value != null)
                {
                    if (Base.NetworkBehaviours != null)
                    {
                        for (int i = 0; i < Base.NetworkBehaviours.Length; i++)
                            NetworkServer.Destroy(Base.NetworkBehaviours[i].gameObject);

                        Base.NetworkBehaviours = null;
                    }

                    Base.NetworkBehaviours = value;

                    for (int x = 0; x < Base.NetworkBehaviours.Length; x++)
                    {
                        Base.NetworkBehaviours[x].netIdentity = Base;
                        Base.NetworkBehaviours[x].ComponentIndex = (byte)x;
                    }
                }
            }
        }

        public Vector3 Position
        {
            get => Transform.position;
            set => Move(value);
        }

        public Vector3 Size
        {
            get => Transform.localScale;
            set => Resize(value);
        }

        public Quaternion Rotation
        {
            get => Transform.rotation;
            set => Rotate(value);
        }

        public void Observe(NetworkConnectionToClient observer)
            => Base.AddObserver(observer);

        public void RemoveObserver(NetworkConnectionToClient observer)
            => Base.RemoveObserver(observer);

        public void Move(Vector3 destination)
        {
            UnSpawn();
            Transform.position = destination;
            Spawn();
        }

        public void Rotate(Quaternion rotation)
        {
            UnSpawn();
            Transform.rotation = rotation;
            Spawn();
        }

        public void Resize(Vector3 size)
        {
            UnSpawn();
            Transform.localScale = size;
            Spawn();
        }

        public void Destroy()
            => NetworkServer.DestroyObject(Base, NetworkServer.DestroyMode.Destroy);

        public void UnSpawn()
            => NetworkServer.DestroyObject(Base, NetworkServer.DestroyMode.Reset);

        public void Spawn()
            => NetworkServer.Spawn(Base.gameObject);

        public void Clear()
        {
            Base.ClearAllComponentsDirtyBits();
            Base.ClearObservers();
        }

        public static Identity Get(NetworkIdentity identity)
        {
            if (identities.TryGetValue(identity.netId, out var apiIdentity))
                return apiIdentity;

            return null;
        }

        public static Identity Get(uint netId)
        {
            if (identities.TryGetValue(netId, out var identity))
                return identity;

            return null;
        }

        internal static void RemoveIdentity(uint netId)
            => identities.Remove(netId);

        internal static void AddIdentity(NetworkIdentity identity)
            => identities[identity.netId] = new Identity(identity);

        internal static void ClearIdentities()
            => identities.Clear();
    }
}