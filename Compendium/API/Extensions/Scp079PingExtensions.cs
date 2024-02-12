using Compendium.API.Roles.Scp079.Abilities;

using PlayerRoles.PlayableScps.Scp079.Pinging;

namespace Compendium.API.Extensions
{
    public static class Scp079PingExtensions
    {
        public static Scp079PingType ToPingType(this byte id)
        {
            if (id > (byte)Scp079PingType.Default)
                return Scp079PingType.None;

            return (Scp079PingType)id;
        }

        public static Scp079PingType ToType(this IPingProcessor pingProcessor)
        {
            if (pingProcessor is null)
                return Scp079PingType.None;

            return (Scp079PingType)(byte)PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility.PingProcessors.IndexOf(pingProcessor);
        }

        public static IPingProcessor ToProcessor(this Scp079PingType type)
        {
            if (type is Scp079PingType.None)
                return null;

            return PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility.PingProcessors[(byte)type];
        }

        public static float ToRange(this Scp079PingType type)
        {
            if (type is Scp079PingType.None)
                return 0f;

            return PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility.PingProcessors[(byte)type].Range;
        }

        public static byte ToIndex(this Scp079PingType type)
            => (byte)type;
    }
}