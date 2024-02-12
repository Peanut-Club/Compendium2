using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerLeftEvent : Event
    {
        public Player Player { get; }

        public bool IsTimeout { get; }

        public PlayerLeftEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}