using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp049;

namespace Compendium.API.Roles.Abilities
{
    public class RagdollAbility<TRole> : Ability<RagdollAbilityBase<TRole>> 
        where TRole : FpcStandardScp
    {
        public RagdollAbility(Player player, RagdollAbilityBase<TRole> ability) : base(player, ability)
        {
            Duration = ability.Duration;
        }

        public float Duration { get; set; }

        public bool InProgress
        {
            get => Base.IsInProgress;
        }

        public bool IsMovementLocked
        {
            get => Base.LockMovement;
        }

        public double CompletionTime
        {
            get => Base._completionTime;
            set
            {
                Base._completionTime = value;
                Base.ServerSendRpc(true);
            }
        }

        public float Progress
        {
            get => Base.ProgressStatus;
        }

        public byte Error
        {
            get => Base._errorCode;
            set
            {
                Base._errorCode = value;
                Base.ServerSendRpc(true);
            }
        }
    }
}