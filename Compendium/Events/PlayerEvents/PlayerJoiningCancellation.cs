using LiteNetLib.Utils;

using System;

namespace Compendium.Events.PlayerEvents
{
    public struct PlayerJoiningCancellation
    {
        public readonly RejectionReason Reason;
        public readonly NetDataWriter Writer;
        public readonly string Message;
        public readonly bool Forced;
        public readonly bool Handled;
        public readonly bool Accepted;
        public readonly ushort Port;
        public readonly long Expiration;
        public readonly byte DelayTime;

        public PlayerJoiningCancellation(RejectionReason reason, NetDataWriter writer, string message, bool forced, bool handled, bool accepted, ushort port, long expiration, byte delay)
        {
            Reason = reason;
            Writer = writer;
            Message = message;
            Forced = forced;
            Handled = handled;
            Port = port;
            Expiration = expiration;
            DelayTime = delay;
        }

        public static PlayerJoiningCancellation Accept()
            => new PlayerJoiningCancellation(RejectionReason.NotSpecified, null, null, false, false, true, 0, 0, 0);

        public static PlayerJoiningCancellation Handle()
            => new PlayerJoiningCancellation(RejectionReason.NotSpecified, null, null, false, true, false, 0, 0, 0);

        public static PlayerJoiningCancellation Delay(byte seconds, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.Delay, null, null, forced, false, true, 0, 0, seconds);

        public static PlayerJoiningCancellation Redirect(ushort port, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.Redirect, null, null, forced, false, true, port, 0, 0);

        public static PlayerJoiningCancellation Reject(string message, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.Custom, null, message, forced, false, false, 0, 0, 0);

        public static PlayerJoiningCancellation Reject(RejectionReason reason, bool forced = true)
            => new PlayerJoiningCancellation(reason, null, null, forced, false, false, 0, 0, 0);

        public static PlayerJoiningCancellation Reject(NetDataWriter writer, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.NotSpecified, writer, null, forced, false, false, 0, 0, 0);

        public static PlayerJoiningCancellation RejectBanned(string reason, DateTime expiery, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.Banned, null, reason, forced, false, false, 0, expiery.Ticks, 0);

        public static PlayerJoiningCancellation RejectBanned(string reason, long expiery, bool forced = true)
            => new PlayerJoiningCancellation(RejectionReason.Banned, null, reason, forced, false, false, 0, expiery, 0);

        internal NetDataWriter GetWriter()
        {
            if (Handled)
                return null;

            if (Reason is RejectionReason.NotSpecified && Writer != null)
                return Writer;

            var writer = new NetDataWriter();

            writer.Put((byte)Reason);

            if (Reason <= RejectionReason.Custom)
            {
                if (Reason != RejectionReason.Banned)
                {
                    if (Reason == RejectionReason.Custom)
                    {
                        writer.Put(Message);
                    }
                }
                else
                {
                    writer.Put(Expiration);
                    writer.Put(Message);
                }
            }
            else if (Reason != RejectionReason.Redirect)
            {
                if (Reason == RejectionReason.Delay)
                {
                    writer.Put(DelayTime);
                }
            }
            else
            {
                writer.Put(Port);
            }

            return writer;
        }
    }
}