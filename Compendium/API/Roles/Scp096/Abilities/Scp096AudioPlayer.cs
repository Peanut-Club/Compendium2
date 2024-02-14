using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp096;

namespace Compendium.API.Roles.Scp096.Abilities
{
    public class Scp096AudioPlayer : AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096AudioPlayer>
    {
        public Scp096AudioPlayer(Player player, PlayerRoles.PlayableScps.Scp096.Scp096AudioPlayer ability) : base(player, ability) { }

        public float PitchRandomization
        {
            get => Base._pitchRandomization;
        }

        public Scp096HitResult SyncedAudio
        {
            get => Base._syncHitSound;
            set => Base._syncHitSound = value;
        }

        public void SendAudio()
            => Base.ServerSendRpc(true);

        public void SendAudio(Scp096HitResult hitResult)
        {
            SyncedAudio = hitResult;
            SendAudio();
        }
    }
}