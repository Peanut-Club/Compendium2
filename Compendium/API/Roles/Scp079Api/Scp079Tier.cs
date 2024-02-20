using Compendium.API.Roles.Abilities;
using Compendium.API.Enums;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079Api
{
    public class Scp079Tier : AbilityWrapper<Scp079TierManager>
    {
        public Scp079Tier(Player player, Scp079TierManager ability) : base(player, ability) { }

        public int Experience
        {
            get => Base.TotalExp;
            set => Base.TotalExp = value;
        }

        public int LevelNumber
        {
            get => Base.AccessTierLevel;
            set => Base.AccessTierIndex = value - 1;
        }

        public int LevelIndex
        {
            get => Base.AccessTierIndex;
            set => Base.AccessTierIndex = value;
        }

        public int Threshold
        {
            get => LevelThresholds[LevelIndex];
            set => LevelThresholds[LevelIndex] = value;
        }

        public int[] LevelThresholds
        {
            get => Base.AbsoluteThresholds;
            set => Base.AbsoluteThresholds = value;
        }

        public Scp079TierType Tier
        {
            get => (Scp079TierType)LevelNumber;
            set => LevelNumber = (int)value;
        }

        public void AddExperience(int experience, Scp079Translation translation = Scp079Translation.CloseDoor, RoleTypeId target = RoleTypeId.None)
            => Base.ServerGrantExperience(experience, (Scp079HudTranslation)translation, target);

        public void ShowNotification(int expAmount, Scp079Translation translation = Scp079Translation.CloseDoor, RoleTypeId target = RoleTypeId.None, bool clearPrevious = true)
        {
            Base._expGainQueue.Enqueue(new Scp079TierManager.ExpQueuedNotification(expAmount, (Scp079HudTranslation)translation, target));
            Base._valueDirty = true;
        }
    }
}