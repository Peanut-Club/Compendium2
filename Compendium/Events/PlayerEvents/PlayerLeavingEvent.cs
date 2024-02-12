using Compendium.API;

using System;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerLeavingEvent : Event
    {
        public static event Action<PlayerLeavingEvent> OnEvent;

        public Player Player { get; }

        public bool IsTimeout { get; }

        public PlayerLeavingEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}