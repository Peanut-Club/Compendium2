using Compendium.API.Roles.Abilities;

using PlayerStatsSystem;

using UnityEngine;

namespace Compendium.API.Roles.Scp173Api.Abilities
{
    public class Scp173SnapAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp173.Scp173SnapAbility>
    {
        public static int Mask { get; } = LayerMask.GetMask("Default", "Hitbox", "Glass", "Door");

        public Scp173SnapAbility(Player player, PlayerRoles.PlayableScps.Scp173.Scp173SnapAbility ability) : base(player, ability)
        {
        }

        public bool IsSpeeding
        {
            get => Base.IsSpeeding;
        }

        public void Snap(Player target)
        {
            var handler = new ScpDamageHandler(Player.Base, DeathTranslations.Scp173);

            if (!target.Base.playerStats.DealDamage(handler))
                return;

            Hitmarker.SendHitmarkerDirectly(Player.Base, 1f);

            if (Base.CastRole.SubroutineModule.TryGetSubroutine<PlayerRoles.PlayableScps.Scp173.Scp173AudioPlayer>(out var audio))
                audio.ServerSendSound(PlayerRoles.PlayableScps.Scp173.Scp173AudioPlayer.Scp173SoundId.Snap);
        }
    }
}