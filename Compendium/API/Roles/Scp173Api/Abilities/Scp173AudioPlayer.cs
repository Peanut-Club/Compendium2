using Compendium.API.Enums;
using Compendium.API.Roles.Abilities;

using UnityEngine;

namespace Compendium.API.Roles.Scp173Api.Abilities
{
    public class Scp173AudioPlayer : AbilityWrapper<PlayerRoles.PlayableScps.Scp173.Scp173AudioPlayer>
    {
        public Scp173AudioPlayer(Player player, PlayerRoles.PlayableScps.Scp173.Scp173AudioPlayer ability) : base(player, ability)
        {

        }

        public Scp173SoundType SyncedAudio
        {
            get => (Scp173SoundType)Base._soundToSend;
            set => Base._soundToSend = (byte)value;
        }

        public Vector3 Position
        {
            get => Base._lastPos;
            set => Base._lastPos = value;
        }

        public void SendAudio()
            => Base.ServerSendRpc(true);

        public void SendAudio(Scp173SoundType type)
        {
            SyncedAudio = type;
            Position = Player.Position;
            SendAudio();
        }
    }
}