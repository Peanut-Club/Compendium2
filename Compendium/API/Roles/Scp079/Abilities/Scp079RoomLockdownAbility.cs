using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp079;

using Interactables.Interobjects.DoorUtils;

using Common.Extensions;

using System.Linq;

using MapGeneration;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079RoomLockdownAbility : AbilityWrapper<Scp079LockdownRoomAbility>
    {
        public Scp079RoomLockdownAbility(Player player, Scp079LockdownRoomAbility ability) : base(player, ability) { }

        public bool IsActive
        {
            get => Base._lockdownInEffect;
            set => Base._lockdownInEffect = value;
        }

        public int MinLevel
        {
            get => Base._minimalTierIndex - 1;
            set => Base._minimalTierIndex = value + 1;
        }

        public float RemainingCooldown
        {
            get => Base.RemainingCooldown;
            set
            {
                Base.RemainingCooldown = value;
                Base.ServerSendRpc(true);
            }
        }

        public float RemainingDuration
        {
            get => Base.RemainingLockdownDuration;
        }

        public float LockdownDuration
        {
            get => Base._lockdownDuration;
            set => Base._lockdownDuration = value;
        }

        public DoorVariant[] LockedDoors
        {
            get => Base._alreadyLockedDown.ToArray();
            set
            {
                if (value is null)
                    return;

                foreach (var door in Base._alreadyLockedDown)
                    door.ServerChangeLock(DoorLockReason.Regular079, false);

                Base._alreadyLockedDown.Clear();
                Base._alreadyLockedDown.AddRange(value);

                foreach (var door in Base._alreadyLockedDown)
                    door.ServerChangeLock(DoorLockReason.Regular079, true);
            }
        }

        public DoorVariant[] TargetDoors
        {
            get => Base._doorsToLockDown.ToArray();
            set
            {
                if (value is null)
                    return;

                Base._doorsToLockDown.Clear();
                Base._doorsToLockDown.AddRange(value);
            }
        }

        public RoomIdentifier LastRoom
        {
            get => Base._lastLockedRoom;
        }

        public void Start()
            => Base.ServerInitLockdown();

        public void Cancel()
        {
            Base._lockdownInEffect = false;
            Base.RemainingCooldown = Base._cooldown;

            foreach (var door in Base._alreadyLockedDown)
                door.ServerChangeLock(DoorLockReason.Regular079, false);

            Base._doorsToLockDown.Clear();
            Base._alreadyLockedDown.Clear();

            Base.ServerSendRpc(true);
        }

        public void Use(RoomIdentifier room, bool removeAux = true, bool setCooldown = true)
        {
            var doors = DoorVariant.AllDoors.Where(d => d != null && d.Rooms != null && d.Rooms.Contains(room));

            Base._lastLockedRoom = room;

            if (removeAux)
                Base.AuxManager.CurrentAux -= Base._cost;

            if (setCooldown)
                Base.RemainingCooldown = Base._lockdownDuration + Base._cooldown;

            foreach (var door in doors)
            {
                if (Base.ValidateDoor(door) && !Base._alreadyLockedDown.Contains(door) && (!door.TargetState || door.GetExactState() >= Base._minStateToClose))
                {
                    door.NetworkTargetState = false;
                    door.ServerChangeLock(DoorLockReason.Regular079, true);

                    if (door == Base._doorLockChanger.LockedDoor)
                        Base._doorLockChanger.ServerUnlock();

                    Base.RewardManager.MarkRooms(door.Rooms);
                    Base._alreadyLockedDown.Add(door);
                }
            }

            Base.ServerSendRpc(true);
        }
    }
}
