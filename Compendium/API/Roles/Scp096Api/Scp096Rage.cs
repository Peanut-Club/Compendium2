using Compendium.API.Core;
using Compendium.API.Enums;

using PlayerRoles.PlayableScps.Scp096;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Roles.Scp096Api
{
    public class Scp096Rage : PlayerWrapper<Scp096RageManager>
    {
        private Scp096RageCycleAbility cycle;
        private Scp096TargetsTracker tracker;

        public Scp096Rage(Player player, Scp096RageManager baseValue) : base(player, baseValue)
        {
            baseValue.GetSubroutine(out cycle);
            baseValue.GetSubroutine(out tracker);

            CalmingShieldMultiplier = 1f;
            EnragedShieldMultiplier = 1f;

            CalmingDuration = 5f;
            RagingDuration = 6.1f;

            MaximumRageDuration = 35f;
            MinimumRageDuration = 20f;

            TimeAddedPerTarget = 15f;

            CanAddDuration = true;
        }

        public IEnumerable<Player> Targets
        {
            get => tracker.Targets.Select(Player.Get);
            set
            {
                tracker.ClearAllTargets();

                foreach (var target in value)
                    tracker.AddTarget(target.Base, false);
            }
        }

        public Scp096RageType State
        {
            get => (Scp096RageType)Base.CastRole.StateController.RageState;
            set => Base.CastRole.StateController.RageState = (Scp096RageState)value;
        }

        public float CalmingShieldMultiplier { get; set; }
        public float EnragedShieldMultiplier { get; set; }

        public float MaximumRageDuration { get; set; }
        public float MinimumRageDuration { get; set; }

        public float TimeAddedPerTarget { get; set; }

        public float CalmingDuration { get; set; }
        public float RagingDuration { get; set; }

        public float EnragedTimeLeft
        {
            get => Base.EnragedTimeLeft;
            set => Base.EnragedTimeLeft = value;
        }

        public float EnragedDuration
        {
            get => Base.TotalRageTime;
            set => Base.TotalRageTime = value;
        }

        public bool IsHumeShieldBlocked
        {
            get => Base.HumeShieldBlocked;
            set => Base.HumeShieldBlocked = value;
        }

        public bool IsDistressed
        {
            get => Base.IsDistressed;
            set
            {
                if (value == IsDistressed)
                    return;

                if (value)
                {
                    State = Scp096RageType.Distressed;
                    return;
                }

                State = Scp096RageType.Docile;
            }
        }

        public bool IsEnraged
        {
            get => Base.IsEnraged;
            set
            {
                if (value == IsEnraged)
                    return;

                if (value)
                {
                    State = Scp096RageType.Enraged;
                    return;
                }

                State = Scp096RageType.Docile;
            }
        }

        public bool IsSendingTargets
        {
            get => tracker._sendTargetsNextFrame;
            set => tracker._sendTargetsNextFrame = value;
        }

        public bool CanAddDuration { get; set; }

        public bool CanStartCycle
        {
            get => cycle.CanStartCycle;
        }

        public bool CanEndCycle
        {
            get => cycle.CanEndCycle;
        }

        public bool CanReceiveTargets
        {
            get => tracker.CanReceiveTargets;
        }

        public void Enrage(float duration = 20f)
            => Base.ServerEnrage(duration);

        public void EndEnrage(bool shouldClearTime = true)
            => Base.ServerEndEnrage(shouldClearTime);

        public void AddDuration(float duration = 3f)
            => Base.ServerIncreaseDuration(duration);

        public bool IsTarget(Player player)
            => tracker.HasTarget(player.Base);

        public bool IsObserving(Player player)
            => tracker.IsObservedBy(player.Base);

        public void AddTarget(Player player)
            => tracker.AddTarget(player.Base, false);

        public void RemoveTarget(Player player)
            => tracker.RemoveTarget(player.Base);

        public void ClearTargets()
            => tracker.ClearAllTargets();

        public void SendTargets()
            => tracker.ServerSendRpc(true);
    }
}