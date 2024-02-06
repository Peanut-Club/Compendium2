using Compendium.API.Extensions;
using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp049.Abilities
{
    public class Scp049SenseAbility : Ability<PlayerRoles.PlayableScps.Scp049.Scp049SenseAbility>
    {
        public Scp049SenseAbility(Player player, PlayerRoles.PlayableScps.Scp049.Scp049SenseAbility scp049SenseAbility) : base(player, scp049SenseAbility)
        {
            AttemptFailedCooldownTime = 2.5f;
            TargetLostCooldownTime = 20f;
            CooldownTime = 40f;
            MaxDistance = 100f;
        }

        public Player Target
        {
            get => Player.Get(Base.Target);
            set
            {
                Base.Target = value?.Base;
                Base.HasTarget = value != null;
                Base.ServerSendRpc(false);
            }
        }

        public float Cooldown
        {
            get => Base.Cooldown.Remaining;
            set
            {
                Base.Cooldown.Remaining = value;
                Base.ServerSendRpc(false);
            }
        }

        public float AttemptFailedCooldownTime { get; set; }
        public float TargetLostCooldownTime { get; set; }
        public float CooldownTime { get; set; }
        public float MaxDistance { get; set; }

        public Player[] GetDeadTargets()
            => Base.DeadTargets.GetPlayerArray();

        public void LoseTarget()
            => Base.ServerLoseTarget();
    }
}