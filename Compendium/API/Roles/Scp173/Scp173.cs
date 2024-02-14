using Common.Values;

using Compendium.API.Enums;
using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp173.Abilities;

using UnityEngine;

namespace Compendium.API.Roles.Scp173
{
    public class Scp173 : SubroutinedRole, IWrapper<PlayerRoles.PlayableScps.Scp173.Scp173Role>
    {
        public Scp173(PlayerRoles.PlayableScps.Scp173.Scp173Role scpRole) : base(scpRole)
        {
            Base = scpRole;

            BreakneckSpeedsAbility = GetRoutine<Scp173BreakneckSpeedsAbility>();
            TeleportAbility = GetRoutine<Scp173TeleportAbility>();
            TantrumAbility = GetRoutine<Scp173TantrumAbility>();
            SnapAbility = GetRoutine<Scp173SnapAbility>();
            AudioPlayer = GetRoutine<Scp173AudioPlayer>();
            Blink = GetRoutine<Scp173Blink>();
        }

        public new PlayerRoles.PlayableScps.Scp173.Scp173Role Base { get; }

        public Scp173BreakneckSpeedsAbility BreakneckSpeedsAbility { get; }
        public Scp173TeleportAbility TeleportAbility { get; }
        public Scp173TantrumAbility TantrumAbility { get; }
        public Scp173SnapAbility SnapAbility { get; }
        public Scp173AudioPlayer AudioPlayer { get; }
        public Scp173Blink Blink { get; }

        public Player TeleportTarget
        {
            get => TeleportAbility.Target;
        }

        public Vector3 TeleportPosition
        {
            get => TeleportAbility.Position;
        }

        public Scp173SoundType SyncedAudio
        {
            get => AudioPlayer.SyncedAudio;
            set => AudioPlayer.SyncedAudio = value;
        }

        public bool IsSpeeding
        {
            get => BreakneckSpeedsAbility.IsActive;
            set => BreakneckSpeedsAbility.IsActive = value;
        }

        public bool IsAiming
        {
            get => TeleportAbility.IsAiming;
            set => TeleportAbility.IsAiming = value;
        }

        public bool WantsToJump
        {
            get => TeleportAbility.WantsToTeleport;
        }

        public void SendAudio()
            => AudioPlayer.SendAudio();

        public void SendAudio(Scp173SoundType type)
            => AudioPlayer.SendAudio(type);

        public void Snap(Player target)
            => SnapAbility.Snap(target);

        public void SpawnTantrum(Vector3 position, Vector3 scale, Quaternion rotation)
            => TantrumAbility.Spawn(position, scale, rotation);

        public void Teleport(Vector3 teleportPosition)
            => TeleportAbility.Blink(teleportPosition);
    }
}