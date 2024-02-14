using Common.Utilities;

using LiteNetLib.Utils;

using System;

namespace Compendium.Events.PlayerEvents
{
    [EventDelegates(typeof(PlayerDelegates))]
    public class PlayerJoiningEvent : CancellableEvent<PlayerJoiningEventCancellation>
    {
        internal override PlayerJoiningEventCancellation AllowedValue => PlayerJoiningEventCancellation.Accept();
        internal override PlayerJoiningEventCancellation CancelledValue => PlayerJoiningEventCancellation.Reject(string.Empty);

        [EventProperty]
        public NetDataReader Reader { get; }

        [EventProperty]
        public DateTime Expiery { get; }

        [EventProperty]
        public string Ip { get; }

        [EventProperty]
        public string Id { get; }

        [EventProperty]
        public string Region { get; }

        [EventProperty]
        public byte[] Signature { get; }

        [EventProperty]
        public int ReaderIndex { get; }

        [EventProperty]
        public bool IsAccepted
        {
            get => IsAllowed.Accepted;
            set => CodeUtils.InlinedElse(value, value == IsAccepted, Accept, () => Reject(string.Empty), null, null);
        }

        [EventProperty]
        public bool IsRejected
        {
            get => !IsAccepted;
            set => IsAccepted = !value;
        }

        public PlayerJoiningEvent(NetDataReader reader, DateTime expiery, string ip, string id, string region, byte[] signature, int readerIndex)
        {
            Reader = reader;
            Expiery = expiery;

            Ip = ip;
            Id = id;

            Region = region;

            Signature = signature;
            ReaderIndex = readerIndex;
        }

        public void Accept()
            => IsAllowed = PlayerJoiningEventCancellation.Accept();

        public void Handle()
            => IsAllowed = PlayerJoiningEventCancellation.Handle();

        public void Delay(byte seconds, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.Delay(seconds, forced);

        public void Redirect(ushort port, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.Redirect(port, forced);

        public void Reject(string message, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.Reject(message, forced);

        public void Reject(NetDataWriter writer, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.Reject(writer, forced);

        public void RejectBanned(string reason, DateTime expiery, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.RejectBanned(reason, expiery, forced);

        public void RejectBanned(string reason, long expieryTicks, bool forced = true)
            => IsAllowed = PlayerJoiningEventCancellation.RejectBanned(reason, expieryTicks, forced);

        public void ResetReader()
        {
            if (Reader != null && Reader.Position != ReaderIndex)
                Reader.SetPosition(ReaderIndex);
        }
    }
}