using Common.Values;

using Compendium.API.Enums;
using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp3114Api.Abilites;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;

namespace Compendium.API.Roles.Scp3114Api
{
    public class Scp3114 : SubroutinedRole, IWrapper<Scp3114Role>
    {
        private Scp3114VoiceLines voiceLines;

        public Scp3114(Scp3114Role scpRole) : base(scpRole)
        {
            Base = scpRole;
            Base.SubroutineModule.TryGetSubroutine(out voiceLines);

            DanceAbility = GetRoutine<Scp3114DanceAbility>();
            DisguiseAbility = GetRoutine<Scp3114DisguiseAbility>();
            StrangleAbility = GetRoutine<Scp3114StrangleAbility>();
        }

        public new Scp3114Role Base { get; }

        public Scp3114DanceAbility DanceAbility { get; }
        public Scp3114DisguiseAbility DisguiseAbility { get; } 
        public Scp3114StrangleAbility StrangleAbility { get; }

        public bool IsDancing
        {
            get => DanceAbility.IsDancing;
            set => DanceAbility.IsDancing = value;
        }

        public bool IsDisguising
        {
            get => DisguiseAbility.IsDisguising;
        }

        public bool IsDisguised
        {
            get => DisguiseAbility.IsDisguised;
        }

        public bool WasDisguised
        {
            get => DisguiseAbility.WasDisguised;
        }

        public byte DisguiseUnitId
        {
            get => DisguiseAbility.UnitId;
            set => DisguiseAbility.UnitId = value;
        }

        public RoleTypeId DisguiseRole
        {
            get => DisguiseAbility.Role;
            set => DisguiseAbility.Role = value;
        }

        public Scp3114DisguiseStatus DisguiseStatus
        {
            get => DisguiseAbility.Status;
            set => DisguiseAbility.Status = value;
        }

        public Scp3114VoiceLine SyncedVoiceLine
        {
            get => (Scp3114VoiceLine)voiceLines._syncName;
            set
            {
                voiceLines._syncName = (byte)value;
                voiceLines.ServerSendRpc(true);
            }
        }

        public Player StrangleTarget
        {
            get => StrangleAbility.Target;
            set => StrangleAbility.Target = value;
        }

        public void Play(Scp3114VoiceLine voiceLine)
            => voiceLines.ServerPlayConditionally((Scp3114VoiceLines.VoiceLinesName)voiceLine);
    }
}