using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    [EventDelegates(typeof(PlayerDelegates))]
    public class PlayerJoinedEvent : Event
    {
        [EventProperty]
        public Player Player { get; }

        public PlayerJoinedEvent(Player player)
            => Player = player;
    }
}