using Compendium.API;

using System;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerLeftEvent : Event
    {
        public static event Action<PlayerLeftEvent> OnEvent;

        public Player Player { get; }

        public bool IsTimeout { get; }

        public PlayerLeftEvent(Player player, bool isTimeout)
        {
            Player = player;
            IsTimeout = isTimeout;
        }
    }
}