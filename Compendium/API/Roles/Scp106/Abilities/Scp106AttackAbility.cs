using Compendium.API.Roles.Abilities;

using Mirror;

using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.Spectating;

namespace Compendium.API.Roles.Scp106.Abilities
{
    public class Scp106AttackAbility : VigorAbility<Scp106Attack>
    {
        public Scp106AttackAbility(Player player, Scp106Attack ability) : base(player, ability)
        {
            Damage = 30f;
            CorrodingDuration = 20f;
        }

        public float Damage { get; set; }
        public float CorrodingDuration { get; set; }

        public float RemainingCooldown
        {
            get => (float)(Base._nextAttack - NetworkTime.time);
            set
            {
                Base._nextAttack = NetworkTime.time + value;
                Base.ServerSendRpc(hub => hub == Base.Owner || Base.Owner.IsSpectatedBy(hub));
            }
        }

        public void ForceAttack()
            => Base.ServerShoot();
    }
}