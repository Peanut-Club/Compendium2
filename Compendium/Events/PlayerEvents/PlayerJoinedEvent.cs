using Compendium.API;

using System;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerJoinedEvent : Event
    {
        public static event Action<PlayerJoiningEvent> OnEvent;

        public Player Player { get; }

        public PlayerJoinedEvent(Player player)
            => Player = player;
    }
}