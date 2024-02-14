using System;

namespace Compendium.Events.PlayerEvents
{
    public static class PlayerDelegates
    {
        public static event Action<PlayerJoiningEvent> OnJoining;
        public static event Action<PlayerJoinedEvent> OnJoined;

        public static event Action<PlayerLeavingEvent> OnLeaving;
        public static event Action<PlayerLeavingEvent> OnLeft;
    }
}