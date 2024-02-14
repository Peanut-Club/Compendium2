using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    [EventDelegates(typeof(PlayerDelegates))]
    public class PlayerLeftEvent : Event
    {
        [EventProperty]
        public Player Player { get; }

        [EventProperty]
        public bool IsTimeout { get; }

        public PlayerLeftEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}