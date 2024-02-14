using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    [EventDelegates(typeof(PlayerDelegates))]
    public class PlayerLeavingEvent : Event
    {
        [EventProperty]
        public Player Player { get; }

        [EventProperty]
        public bool IsTimeout { get; }

        public PlayerLeavingEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}