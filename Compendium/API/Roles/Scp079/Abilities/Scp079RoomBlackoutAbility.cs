using Compendium.API.Roles.Abilities;
using Compendium.API.Utilities;

using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles;

using Mirror;

using System.Collections.Generic;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079RoomBlackoutAbility : Ability<Scp079BlackoutRoomAbility>
    {
        public Scp079RoomBlackoutAbility(Player player, Scp079BlackoutRoomAbility ability) : base(player, ability) { }

        public int MaxCapacity
        {
            get => Base.CurrentCapacity;
            set => Base._capacityPerTier[Base.TierManager.AccessTierIndex] = value;
        }

        public Dictionary<uint, double> Cooldowns
        {
            get => Base._blackoutCooldowns;
        }

        public void PlayConfirmation(RoomLightController roomLightController)
        {
            var writer = NetworkWriterPool.Get();

            writer.WriteNetworkBehaviour(roomLightController);
            writer.WriteByte((byte)Base.RoomsOnCooldown);

            foreach (var pair in Base._blackoutCooldowns)
            {
                writer.WriteUInt(pair.Key);
                writer.WriteDouble(pair.Value);
            }

            SubroutineUtils.ServerSendSync<Scp079BlackoutRoomAbility>(Player.Base, Player.Base, writer, RoleTypeId.Scp079, Base.SyncIndex);
        }
    }
}