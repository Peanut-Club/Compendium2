using Compendium.API.Extensions;
using Compendium.API.Interfaces;
using Compendium.API.Enums;

using Compendium.API.Roles.Abilities;

using UnityEngine;

using RelativePositioning;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079PingAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility>
    {
        public Scp079PingAbility(Player player, PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility ability) : base(player, ability) { }

        public int ExpCost
        {
            get => Base._cost;
            set => Base._cost = value;
        }

        public Scp079PingType SyncedPing
        {
            get => Base._syncProcessorIndex.ToPingType();
        }

        public void Ping(IWorldObject worldObject, bool removeAux = true)
        {
            if (worldObject is null)
                return;

            Ping(worldObject.Position, worldObject.ObjectType switch
            {
                _ => Scp079PingType.Default
            }, removeAux);
        }

        public void Ping(Vector3 position, Scp079PingType pingType, bool removeAux = true)
        {
            Base._syncProcessorIndex = pingType.ToIndex();
            Base._syncNormal = position;
            Base._syncPos = new RelativePosition(position);

            if (removeAux)
                Base.AuxManager.CurrentAux -= Base._cost;

            Base.ServerSendRpc(true);
        }
    }
}