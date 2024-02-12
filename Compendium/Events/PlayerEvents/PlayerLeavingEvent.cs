using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerLeavingEvent : Event
    {
        public Player Player { get; }

        public bool IsTimeout { get; }

        public PlayerLeavingEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}