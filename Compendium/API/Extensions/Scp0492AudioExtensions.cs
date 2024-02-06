using Compendium.API.Roles.Scp0492.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Extensions
{
    public static class Scp0492AudioExtensions
    {
        public static Scp0492AudioType ToZombieAudioType(this ZombieAudioPlayer.Scp0492SoundId scp0492SoundId)
            => (Scp0492AudioType)(byte)scp0492SoundId;

        public static Scp0492AudioType ToZombieAudioType(this byte soundId)
            => (Scp0492AudioType)soundId;

        public static ZombieAudioPlayer.Scp0492SoundId ToZombieSoundId(this Scp0492AudioType scp0492AudioType)
            => (ZombieAudioPlayer.Scp0492SoundId)(byte)scp0492AudioType;

        public static byte ToZombieAudioByte(this Scp0492AudioType scp0492AudioType)
            => (byte)scp0492AudioType;
    }
}