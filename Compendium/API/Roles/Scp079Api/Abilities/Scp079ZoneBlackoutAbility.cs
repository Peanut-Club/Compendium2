using Compendium.API.Roles.Abilities;
using Compendium.API.Enums;

using MapGeneration;

using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079ZoneBlackoutAbility : AbilityWrapper<Scp079BlackoutZoneAbility>
    {
        public Scp079ZoneBlackoutAbility(Player player, Scp079BlackoutZoneAbility ability) : base(player, ability) { }

        public int ExpCost
        {
            get => Base._cost;
            set => Base._cost = value;
        }

        public int UnlockLevel
        {
            get => Base._minTierIndex + 1;
            set => Base._minTierIndex = value - 1;
        }

        public float FlickerDuration
        {
            get => Base._duration;
            set => Base._duration = value;
        }

        public float RemainingCooldown
        {
            get => Base._cooldownTimer.Remaining;
            set
            {
                Base._cooldownTimer.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public float Cooldown
        {
            get => Base._cooldown;
            set => Base._cooldown = value;
        }

        public bool IsUnlocked
        {
            get => Base.Unlocked;
        }

        public FacilityZone[] AvailableZones
        {
            get => Base._availableZones;
            set => Base._availableZones = value;
        }

        public FacilityZone ActiveZone
        {
            get => Base._syncZone;
            set
            {
                Base._syncZone = value;
                Base.ServerSendRpc(true);
            }
        }

        public Scp079Translation Error
        {
            get => (Scp079Translation)Base.ErrorCode;
        }

        public void Blackout(FacilityZone facilityZone, bool triggerCooldown = true, bool removeAux = true)
        {
            Base._syncZone = facilityZone;

            foreach (var light in RoomLightController.Instances)
            {
                if (light.Room != null && light.Room.Zone == facilityZone)
                    light.ServerFlickerLights(FlickerDuration);
            }

            if (triggerCooldown)
                Base._cooldownTimer.Trigger(Base._cooldown);

            if (removeAux && Base._cost > 0)
                Base.AuxManager.CurrentAux -= Base._cost;

            Base.ServerSendRpc(true);
        }
    }
}