using Compendium.Components;
using Compendium.Players;
using Compendium.Utilities;

using Mirror;

namespace Compendium
{
    public class Player : Disposable
    {
        public PlayerNickname Nickname { get; }
        public PlayerUserId UserId { get; set; }

        public ComponentContainer Components { get; }

        public NetworkIdentity Identity { get; }
        public NetworkConnectionToClient Connection { get; }

        public int PlayerId { get; set; }
        public uint NetId { get; }
    }
}