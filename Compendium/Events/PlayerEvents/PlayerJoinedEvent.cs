using Compendium.API;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerJoinedEvent : Event
    {
        public Player Player { get; }

        public PlayerJoinedEvent(Player player)
            => Player = player;
    }
}