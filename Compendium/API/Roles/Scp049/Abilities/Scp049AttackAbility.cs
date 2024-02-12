using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp049.Abilities
{
    public class Scp049AttackAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp049.Scp049AttackAbility>
    {
        public const float DefaultCardiacArrestDuration = 20f;

        public Scp049AttackAbility(Player player, PlayerRoles.PlayableScps.Scp049.Scp049AttackAbility scp049AttackAbility) : base(player, scp049AttackAbility)
        {
            CooldownTime = PlayerRoles.PlayableScps.Scp049.Scp049AttackAbility.CooldownTime;
            AttackDistance = PlayerRoles.PlayableScps.Scp049.Scp049AttackAbility.AttackDistance;

            CardiacArrestDuration = DefaultCardiacArrestDuration;
        }

        public Player Target
        {
            get => Player.Get(Base._target);
            set => Base._target = value.Base;
        }

        public bool IsInstantKill
        {
            get => Base._isInstaKillAttack;
            set => Base._isInstaKillAttack = value;
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

        public float CooldownTime { get; set; }
        public float AttackDistance { get; set; }
        public float CardiacArrestDuration { get; set; }
    }
}