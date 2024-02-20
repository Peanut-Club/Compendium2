using Common.Values;

using Compendium.API.GameModules.Subroutines;
using Compendium.API.Enums;

using Compendium.API.Roles.Interfaces;
using Compendium.API.Roles.Scp079Api.Abilities;

using PlayerRoles;
using PlayerRoles.Subroutines;
using PlayerRoles.PlayableScps.Scp079;

using Scp079TeslaAbility = Compendium.API.Roles.Scp079Api.Abilities.Scp079TeslaAbility;
using Scp079SpeakerAbility = Compendium.API.Roles.Scp079Api.Abilities.Scp079SpeakerAbility;

namespace Compendium.API.Roles.Scp079Api
{
    public class Scp079 : Role, ISubroutineRole, IWrapper<Scp079Role>
    {
        public Scp079(Scp079Role roleBase) : base(roleBase)
        {
            Base = roleBase;

            Subroutines = Player.Get<SubroutineManager>();
            Subroutines.Initialize(((ISubroutinedRole)roleBase).SubroutineModule, Player);

            Aux = GetRoutine<Scp079Aux>();
            Tier = GetRoutine<Scp079Tier>();
            Signal = GetRoutine<Scp079Signal>();
            Camera = GetRoutine<Scp079CameraManager>();
            MapAbility = GetRoutine<Scp079MapAbility>();
            PingAbility = GetRoutine<Scp079PingAbility>();
            TeslaAbility = GetRoutine<Scp079TeslaAbility>();
            SpeakerAbility = GetRoutine<Scp079SpeakerAbility>();
            DoorLockAbility = GetRoutine<Scp079DoorLockAbility>();
            ScannerTeamFilter = GetRoutine<Scp079ScannerTeamFilter>();
            ScannerZoneFilter = GetRoutine<Scp079ScannerZoneFilter>();
            ElevatorUseAbility = GetRoutine<Scp079ElevatorUseAbility>();
            RoomBlackoutAbility = GetRoutine<Scp079RoomBlackoutAbility>();
            RoomLockdownAbility = GetRoutine<Scp079RoomLockdownAbility>();
            ZoneBlackoutAbility = GetRoutine<Scp079ZoneBlackoutAbility>();
        }

        public new Scp079Role Base { get; }

        public SubroutineManager Subroutines { get; }

        public Scp079MapAbility MapAbility { get; }
        public Scp079PingAbility PingAbility { get; }
        public Scp079TeslaAbility TeslaAbility { get; }
        public Scp079SpeakerAbility SpeakerAbility { get; }
        public Scp079DoorLockAbility DoorLockAbility { get; }
        public Scp079ElevatorUseAbility ElevatorUseAbility { get; }
        public Scp079RoomBlackoutAbility RoomBlackoutAbility { get; }
        public Scp079RoomLockdownAbility RoomLockdownAbility { get; }
        public Scp079ZoneBlackoutAbility ZoneBlackoutAbility { get; }

        public Scp079ScannerTeamFilter ScannerTeamFilter { get; }
        public Scp079ScannerZoneFilter ScannerZoneFilter { get; }

        public Scp079CameraManager Camera { get; }
        public Scp079Signal Signal { get; }
        public Scp079Tier Tier { get; }
        public Scp079Aux Aux { get; }

        public Camera CurrentCamera
        {
            get => Camera.Camera;
            set => Camera.Camera = value;
        }

        public Scp079TierType Level
        {
            get => Tier.Tier;
            set => Tier.Tier = value;
        }

        public Scp079PingType SyncedPing
        {
            get => PingAbility.SyncedPing;
        }

        public int LevelNumber
        {
            get => Tier.LevelNumber;
            set => Tier.LevelNumber = value;
        }

        public int Experience
        {
            get => Tier.Experience;
            set => Tier.Experience = value;
        }

        public float AuxCurrent
        {
            get => Aux.Current;
            set => Aux.Current = value;
        }

        public float AuxRegeneration
        {
            get => Aux.Regeneration;
            set => Aux.Regeneration = value;
        }

        public float AuxMaximum
        {
            get => Aux.Maximum;
            set => Aux.Maximum = value;
        }

        public bool IsMapOpened
        {
            get => MapAbility.IsOpened;
        }

        public bool IsSignalLost
        {
            get => Signal.IsLost;
        }

        public void RegenerateAux()
            => Aux.Regenerate();

        public void RegenerateFullAux()
            => Aux.RegenerateFull();

        public void AddExperience(int exp, Scp079Translation reason = Scp079Translation.CloseDoor, RoleTypeId target = RoleTypeId.None)
            => Tier.AddExperience(exp, reason, target);

        public void ShowNotification(int expAmount, Scp079Translation translation = Scp079Translation.CloseDoor, RoleTypeId target = RoleTypeId.None)
            => Tier.ShowNotification(expAmount, translation, target);

        #region Subroutines
        public TRoutine AddRoutine<TRoutine>() where TRoutine : ICustomSubroutine
            => Subroutines.Add<TRoutine>();

        public TRoutine GetRoutine<TRoutine>() where TRoutine : ISubroutine
            => Subroutines.Get<TRoutine>();

        public bool RemoveRoutine<TRoutine>() where TRoutine : ICustomSubroutine
            => Subroutines.Remove<TRoutine>();

        public bool TryGetRoutine<TRoutine>(out TRoutine routine) where TRoutine : ISubroutine
            => Subroutines.TryGet(out routine);
        #endregion
    }
}