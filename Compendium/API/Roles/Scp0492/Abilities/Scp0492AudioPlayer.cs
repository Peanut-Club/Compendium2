using Compendium.API.Extensions;
using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492.Abilities
{
    public class Scp0492AudioPlayer : Ability<ZombieAudioPlayer>
    {
        public Scp0492AudioPlayer(Player player, ZombieAudioPlayer ability) : base(player, ability) { }

        public Scp0492AudioType SoundToSend
        {
            get => Base._soundToSend.ToZombieAudioType();
            set => Base._soundToSend = value.ToZombieAudioByte();
        }

        public void Play(Scp0492AudioType audioType)
        {
            SoundToSend = audioType;
            Base.ServerSendRpc(true);
        }

        public void Growl()
            => Play(Scp0492AudioType.Growl);

        public void AngryGrowl()
            => Play(Scp0492AudioType.AngryGrowl);
    }
}