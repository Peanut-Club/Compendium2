using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp096Api.Abilities
{
    public class Scp096ChargeAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096ChargeAbility>
    {
        public Scp096ChargeAbility(Player player, PlayerRoles.PlayableScps.Scp096.Scp096ChargeAbility ability) : base(player, ability)
        {
            Cooldown = 5f;
            Duration = 1f;

            NonTargetDamage = 35f;
            TargetDamage = 90f;
            ObjectDamage = 750f;
        }

        public float Cooldown { get; set; }
        public float Duration { get; set; }

        public float NonTargetDamage { get; set; }
        public float TargetDamage { get; set; }
        public float ObjectDamage { get; set; }

        public float RemainingCooldown
        {
            get => Base.Cooldown.Remaining;
            set
            {
                Base.Cooldown.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public float RemainingDuration
        {
            get => Base.Duration.Remaining;
            set
            {
                Base.Duration.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public bool CanCharge
        {
            get => Base.CanCharge;
        }

        public void ForceCharge()
        {
            Base._hitHandler.Clear();
            Base.Duration.Trigger(Duration);
            Base.CastRole.StateController.SetAbilityState(PlayerRoles.PlayableScps.Scp096.Scp096AbilityState.Charging);
            Base.ServerSendRpc(true);
        }
    }
}
