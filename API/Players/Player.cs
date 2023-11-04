using Compendium.Components;
using Compendium.Utilities;

using Mirror;

using UnityEngine;

using System;

namespace Compendium.Players
{
    /// <summary>
    /// A class used for managing players, essentially a wrapper for <see cref="ReferenceHub"/>.
    /// </summary>
    public class Player : 
        Disposable, 
        
        IEquatable<Player>, 
        IEquatable<ReferenceHub>, 
        IEquatable<GameObject>
    {
        private PlayerUserId pUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="playerHub">The <see cref="ReferenceHub"/> to use.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Player(ReferenceHub playerHub)
        {
            if (playerHub is null)
                throw new ArgumentNullException(nameof(playerHub));

            Identity = playerHub.netIdentity;
            Connection = playerHub.connectionToClient;
            Hub = playerHub;

            Components = new ComponentContainer(playerHub.gameObject);

            pUserId = new PlayerUserId(Hub.authManager.UserId);

            NetId = playerHub.netId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to use.</param>
        public Player(GameObject gameObject)
            : this(ReferenceHub.GetHub(gameObject)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="playerId">The ID of the player's <see cref="ReferenceHub"/> to use.</param>
        public Player(int playerId) 
            : this(ReferenceHub.GetHub(playerId)) { }

        /// <summary>
        /// Gets a container with all components present on the player's <see cref="GameObject"/>
        /// </summary>
        public ComponentContainer Components { get; private set; }

        /// <summary>
        /// Gets the player's <see cref="NetworkIdentity"/> component.
        /// </summary>
        public NetworkIdentity Identity { get; private set; }

        /// <summary>
        /// Gets the player's network connection component.
        /// </summary>
        public NetworkConnectionToClient Connection { get; private set; }

        /// <summary>
        /// Gets the player's <see cref="ReferenceHub"/> component.
        /// </summary>
        public ReferenceHub Hub { get; private set; }

        /// <summary>
        /// Gets or sets the player's user ID.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public PlayerUserId UserId
        {
            get => pUserId ??= new PlayerUserId(Hub.authManager.UserId);
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                pUserId = value;
                Hub.authManager.UserId = value.Value;
            }
        }

        /// <summary>
        /// Gets or sets the player's nickname.
        /// </summary>
        public string Nickname { get => Hub.nicknameSync.Network_myNickSync; set => Hub.nicknameSync.Network_myNickSync = value; }

        /// <summary>
        /// Gets or sets the player's display name.
        /// </summary>
        public string DisplayName { get => Hub.nicknameSync.Network_displayName; set => Hub.nicknameSync.Network_displayName = value; }

        /// <summary>
        /// Gets or sets the player's player ID.
        /// </summary>
        public int PlayerId { get => Hub.PlayerId; set => Hub.Network_playerId = new RecyclablePlayerId(value); }

        /// <summary>
        /// Gets or sets the player's network ID.
        /// </summary>
        public uint NetId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this player has disconnected from the server.
        /// </summary>
        public bool IsDisconnected { get; private set; }

        /// <summary>
        /// Gets a <see cref="Boolean"/> indicating whether or not this instance is equal to a specific <see cref="Player"/>.
        /// </summary>
        /// <param name="other">The <see cref="Player"/> to compare to.</param>
        /// <returns>true if the <see cref="Player"/> is equal to this <see cref="Player"/> instance, othewise false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Equals(Player other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            return NetId == other.NetId;
        }

        /// <summary>
        /// Gets a <see cref="Boolean"/> indicating whether or not this instance is equal to a specific <see cref="ReferenceHub"/>.
        /// </summary>
        /// <param name="other">The <see cref="ReferenceHub"/> to compare to.</param>
        /// <returns>true if the <see cref="ReferenceHub"/> is equal to this <see cref="Player"/> instance, othewise false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Equals(ReferenceHub other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            return Hub.netId == other.netId;
        }

        /// <summary>
        /// Gets a <see cref="Boolean"/> indicating whether or not this instance is equal to a specific <see cref="GameObject"/>.
        /// </summary>
        /// <param name="other">The <see cref="GameObject"/> to compare to.</param>
        /// <returns>true if the <see cref="GameObject"/> is equal to this <see cref="Player"/> instance, othewise false.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Equals(GameObject other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            var hub = ReferenceHub.GetHub(other);

            if (hub is null)
                return false;

            return Equals(hub);
        }

        /// <summary>
        /// Removes all references, but keeps identification properties. 
        /// <b>ONLY USE ONCE WHEN A PLAYER DISCONNECTS!</b>
        /// </summary>
        public override void DisposeInternal()
        {
            base.DisposeInternal();

            Components.DisposeInternal();
            Components = null;

            Hub = null;
            Identity = null;
            Connection = null;

            NetId = 0;

            IsDisconnected = true;
        }
    }
}