using Common.Utilities;

using LiteNetLib.Utils;

using System;

namespace Compendium.Events.PlayerEvents
{
    public class PlayerJoiningEvent : CancellableEvent<PlayerJoiningCancellation>
    {
        public static event Action<PlayerJoiningEvent> OnEvent;

        public NetDataReader Reader { get; }

        public DateTime Expiery { get; }

        public string Ip { get; }
        public string Id { get; }

        public string Region { get; }

        public byte[] Signature { get; }

        public int ReaderIndex { get; }

        public bool IsAccepted
        {
            get => IsAllowed.Accepted;
            set => CodeUtils.InlinedElse(value, value == IsAccepted, Accept, () => Reject(string.Empty), null, null);
        }

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
            => IsAllowed = PlayerJoiningCancellation.Accept();

        public void Handle()
            => IsAllowed = PlayerJoiningCancellation.Handle();

        public void Delay(byte seconds, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.Delay(seconds, forced);

        public void Redirect(ushort port, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.Redirect(port, forced);

        public void Reject(string message, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.Reject(message, forced);

        public void Reject(NetDataWriter writer, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.Reject(writer, forced);

        public void RejectBanned(string reason, DateTime expiery, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.RejectBanned(reason, expiery, forced);

        public void RejectBanned(string reason, long expieryTicks, bool forced = true)
            => IsAllowed = PlayerJoiningCancellation.RejectBanned(reason, expieryTicks, forced);

        public void RestoreReader()
        {
            if (Reader != null && Reader.Position != ReaderIndex)
                Reader.SetPosition(ReaderIndex);
        }
    }
}