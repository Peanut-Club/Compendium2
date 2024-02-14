using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp096;
using PlayerStatsSystem;

namespace Compendium.API.Roles.Scp096.Abilities
{
    public class Scp096AttackAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096AttackAbility>
    {
        public Scp096AttackAbility(Player player, PlayerRoles.PlayableScps.Scp096.Scp096AttackAbility ability) : base(player, ability)
        {
            Cooldown = 0.5f;
            DoorDamage = 250f;
            HumanDamage = 60f;
            WindowDamage = 500f;
        }

        public bool CanAttack
        {
            get => Base.AttackPossible;
        }

        public bool IsLeftHand
        {
            get => Base.LeftAttack;
            set => Base.LeftAttack = value;
        }

        public float RemainingCooldown
        {
            get => Base._serverAttackCooldown.Remaining;
            set => Base._serverAttackCooldown.Remaining = value;
        }

        public float Cooldown { get; set; }

        public float DoorDamage { get; set; }
        public float HumanDamage { get; set; }
        public float WindowDamage { get; set; }

        public void ForceAttack(bool isLeftHandAttack = true)
        {
            IsLeftHand = isLeftHandAttack;

            Base.CastRole.StateController.SetAbilityState(Scp096AbilityState.Attacking);

            var hit = IsLeftHand ? Base._leftHitHandler : Base._rightHitHandler;

            hit.Clear();

            Base._hitResult = hit.DamageSphere(Base.Owner.PlayerCameraReference.position + Base.Owner.PlayerCameraReference.forward * Base._sphereHitboxOffset, Base._sphereHitboxRadius);
            Base._audioPlayer.ServerPlayAttack(Base._hitResult);

            Base.ServerSendRpc(true);
        }

        public void ForceAttack(Player target, float damage, bool isLeftHandAttack = true)
        {
            if (target is null)
                return;

            if (damage < 0f)
                damage = float.MaxValue;
            
            var handler = new Scp096DamageHandler((Player.Role as Scp096).Base, damage, isLeftHandAttack ? Scp096DamageHandler.AttackType.SlapLeft : Scp096DamageHandler.AttackType.SlapRight);
            var result = handler.ApplyDamage(target.Base);

            Base._hitResult = result switch
            {
                DamageHandlerBase.HandlerOutput.Damaged => Scp096HitResult.Human,
                DamageHandlerBase.HandlerOutput.Death   => Scp096HitResult.Lethal,

                _ => Scp096HitResult.None
            };

            Base._audioPlayer.ServerPlayAttack(Base._hitResult);
            Base.ServerSendRpc(true);
        }
    }
}