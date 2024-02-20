using Common.Utilities;

using Compendium.API.Roles.Abilities;

using Interactables.Interobjects.DoorUtils;

using Mirror;

using PlayerRoles.PlayableScps.Scp079;

using System;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079DoorLockAbility : AbilityWrapper<Scp079DoorLockChanger>
    {
        public Scp079DoorLockAbility(Player player, Scp079DoorLockChanger ability) : base(player, ability) { }

        public float RemainingCooldown
        {
            get => Base._cooldown.Remaining;
            set
            {
                Base._cooldown.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public float BaseCooldown
        {
            get => Base._cooldownBaseline;
            set => Base._cooldownBaseline = value;
        }

        public float CostPerSecond
        {
            get => Base._lockCostPerSec;
            set => Base._lockCostPerSec = value;
        }

        public int OpenLockedDoorCost
        {
            get => Base.LockOpenDoorCost;
            set => Base.LockOpenDoorCost = value;
        }

        public int CloseLockedDoorCost
        {
            get => Base.LockClosedDoorCost;
        }

        public DoorVariant LockedDoor
        {
            get => Base.LockedDoor;
            set => CodeUtils.InlinedElse(value != null, true, () => Lock(value), Unlock, null, null);
        }

        public DoorVariant FailedDoor
        {
            get => Base._failedDoor;
        }

        public TimeSpan LockTime
        {
            get => TimeSpan.FromSeconds(NetworkTime.time - Base._lockTime);
        }

        public void Lock(DoorVariant door, bool removeAux = true)
        {
            Base._lockTime = NetworkTime.time;

            Base.LastDoor = door;

            Base.LockedDoor = door;
            Base.LockedDoor.ServerChangeLock(DoorLockReason.Regular079, true);

            Base.RewardManager.MarkRooms(Base.LastDoor.Rooms);

            if (removeAux)
                Base.AuxManager.CurrentAux -= Base.GetCostForDoor(DoorAction.Locked, door);
        }

        public void Unlock()
            => Base.ServerUnlock();
    }
}