using Compendium.API.Extensions;
using Compendium.API.Inventory;
using Compendium.API.Roles;
using PlayerRoles.FirstPersonControl;

using System.Collections.Generic;

using UnityEngine;

namespace Compendium.API
{
    public class Player
    {
        private static readonly HashSet<Player> playerSet = new HashSet<Player>();

        public static IReadOnlyCollection<Player> List => playerSet;

        public static Player Get(ReferenceHub hub)
        {
            foreach (var ply in playerSet)
            {
                if (ply.Hub.netId == hub.netId)
                    return ply;
            }

            return null;
        }

        public Player(ReferenceHub hub)
        {
            Hub = hub;

            Inventory = new InventoryManager(this);
            Role = new RoleManager(this);

            playerSet.Add(this);
        }

        public ReferenceHub Hub { get; }

        public InventoryManager Inventory { get; }
        public RoleManager Role { get; }
        public UserId UserId { get; set; }

        public string Nick { get; set; }

        public Vector3 Position
        {
            get => Hub.GetRealPosition();
            set => Hub.TryOverridePosition(Position, Rotation.eulerAngles);
        }

        public Quaternion Rotation
        {
            get => Hub.GetRealRotation();
            set => Hub.TryOverridePosition(Position, value.eulerAngles);
        }
    }
}