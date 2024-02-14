using Common.Extensions;
using Common.Pooling.Pools;
using Common.Values;

using Compendium.API.Modules;

using Compendium.API.Roles.Abilities;

using Compendium.API.Roles.Scp0492.Abilities;

using Compendium.API.Roles.Scp079;
using Compendium.API.Roles.Scp079.Abilities;

using Compendium.API.Roles.Scp096.Abilities;
using Compendium.API.Roles.Scp106;
using Compendium.API.Roles.Scp106.Abilities;
using Compendium.API.Roles.Scp173;
using Compendium.API.Roles.Scp173.Abilities;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp049.Zombies;

using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.Map;

using PlayerRoles.Subroutines;

using System;
using System.Collections.Generic;

using Scp079SpeakerAbility = Compendium.API.Roles.Scp079.Abilities.Scp079SpeakerAbility;
using Scp079TeslaAbility = Compendium.API.Roles.Scp079.Abilities.Scp079TeslaAbility;

namespace Compendium.API.GameModules.Subroutines
{
    public class SubroutineManager : ModuleBase, IWrapper<SubroutineManagerModule>
    {
        private static readonly ModuleUpdate UpdateInfo = new ModuleUpdate("OnUpdate", 15, false, true);

        public override string Name { get; } = "Subroutine Manager";

        public SubroutineManagerModule Base { get; private set; }
        public Player Player { get; private set; }

        public IReadOnlyList<ISubroutine> Subroutines { get; private set; }

        public int SubroutineCount
        {
            get => Subroutines.Count;
        }

        public override ModuleUpdate OnStart()
            => UpdateInfo;

        public override bool IsValid()
            => IsActive && IsUpdateActive && Player != null && Base != null && Subroutines != null && Subroutines.Count > 0;

        public TRoutine Add<TRoutine>() where TRoutine : ICustomSubroutine
        {
            if (TryGet<TRoutine>(out var customRoutine))
                return customRoutine;

            customRoutine = typeof(TRoutine).Construct<TRoutine>();

            customRoutine.Player = Player;
            customRoutine.Previous = Player;
            customRoutine.Start();

            AppendRoutine(customRoutine);

            return customRoutine;
        }

        public bool Remove<TRoutine>() where TRoutine : ICustomSubroutine
        {
            if (!TryGet<TRoutine>(out var customRoutine))
                return false;

            customRoutine.Destroy();

            customRoutine.Player = null;
            customRoutine.Previous = null;

            RemoveRoutine(customRoutine);

            return true;
        }

        public TRoutine Get<TRoutine>() where TRoutine : ISubroutine
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is TRoutine tRoutine)
                    return tRoutine;
            }

            return default;
        }

        public bool TryGet<TRoutine>(out TRoutine routine) where TRoutine : ISubroutine
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is TRoutine tRoutine)
                {
                    routine = tRoutine;
                    return true;
                }
            }

            routine = default;
            return false;
        }

        public override void OnStop()
        {
            DestroyCustomRoutines();

            Subroutines = null;
            Player = null;
            Base = null;
        }

        internal void DestroyCustomRoutines()
        {
            if (Subroutines != null)
            {
                for (int i = 0; i < Subroutines.Count; i++)
                {
                    if (Subroutines[i] is ICustomSubroutine customSubroutine
                        && customSubroutine.Player != null)
                    {
                        try
                        {
                            customSubroutine.Destroy();

                            customSubroutine.Player = null;
                            customSubroutine.Previous = null;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"An error occured while attempting to stop subroutine '{customSubroutine.GetType().FullName}':\n{ex}");
                        }
                    }
                }
            }
        }

        internal void Initialize(SubroutineManagerModule subroutineManagerModule, Player player)
        {
            DestroyCustomRoutines();

            Base = subroutineManagerModule;
            Player = player;

            if (subroutineManagerModule != null)
            {
                var subroutines = ListPool<ISubroutine>.Shared.Rent();

                for (int i = 0; i < subroutineManagerModule.AllSubroutines.Length; i++)
                {
                    var subroutine = GetSubroutine(subroutineManagerModule.AllSubroutines[i]);

                    if (subroutine != null)
                        subroutines.Add(subroutine);
                }

                Subroutines = subroutines.AsReadOnly();

                ListPool<ISubroutine>.Shared.Return(subroutines);
            }
            else
            {
                Subroutines = new List<ISubroutine>();
            }
        }

        private void AppendRoutine(ISubroutine subroutine)
        {
            var routines = ListPool<ISubroutine>.Shared.Rent(Subroutines);

            routines.Add(subroutine);

            Subroutines = routines.AsReadOnly();

            ListPool<ISubroutine>.Shared.Return(routines);
        }

        private void RemoveRoutine(ISubroutine subroutine)
        {
            var routines = ListPool<ISubroutine>.Shared.Rent(Subroutines);

            routines.Remove(subroutine);

            Subroutines = routines.AsReadOnly();

            ListPool<ISubroutine>.Shared.Return(routines);
        }

        private void OnUpdate()
        {
            for (int i = 0; i < SubroutineCount; i++)
            {
                if (Subroutines[i] is ICustomSubroutine customSubroutine)
                {
                    try
                    {
                        if (customSubroutine.Player is null || customSubroutine.Player.NetworkId != Player.NetworkId)
                        {
                            customSubroutine.Previous = customSubroutine.Player;
                            customSubroutine.Player = Player;
                            customSubroutine.OnOwnerChanged();
                        }

                        customSubroutine.Update();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"An exception has been caught while updating subroutine '{customSubroutine.GetType().Name}':\n{ex}");
                    }
                }
            }
        }

        private ISubroutine GetSubroutine(SubroutineBase subroutineBase)
        {
            switch (subroutineBase)
            {
                #region SCP-049
                case Scp049AttackAbility scp049AttackAbility:
                    return new Roles.Scp049.Abilities.Scp049AttackAbility(Player, scp049AttackAbility);

                case Scp049CallAbility scp049CallAbility:
                    return new Roles.Scp049.Abilities.Scp049CallAbility(Player, scp049CallAbility);

                case Scp049ResurrectAbility scp049ResurrectAbility:
                    return new Roles.Scp049.Abilities.Scp049ResurrectAbility(Player, scp049ResurrectAbility);

                case Scp049SenseAbility scp049SenseAbility:
                    return new Roles.Scp049.Abilities.Scp049SenseAbility(Player, scp049SenseAbility);

                case Scp049ResurrectIndicators scp049ResurrectIndicators:
                    return new AbilityWrapper<Scp049ResurrectIndicators>(Player, scp049ResurrectIndicators);
                #endregion

                #region SCP-049-2
                case ZombieAttackAbility zombieAttackAbility:
                    return new AbilityWrapper<ZombieAttackAbility>(Player, zombieAttackAbility);

                case ZombieConsumeAbility zombieConsumeAbility:
                    return new AbilityWrapper<ZombieConsumeAbility>(Player, zombieConsumeAbility);

                case ZombieAudioPlayer zombieAudioPlayer:
                    return new Scp0492AudioPlayer(Player, zombieAudioPlayer);

                case ZombieBloodlustAbility zombieBloodlustAbility:
                    return new Scp0492BloodlustAbility(Player, zombieBloodlustAbility);

                case ZombieIndicatorTracker zombieIndicatorTracker:
                    return new AbilityWrapper<ZombieIndicatorTracker>(Player, zombieIndicatorTracker);
                #endregion

                #region SCP-079
                case Scp079AuxManager auxManager:
                    return new Scp079Aux(Player, auxManager);

                case Scp079BlackoutRoomAbility scp079BlackoutRoomAbility:
                    return new Scp079RoomBlackoutAbility(Player, scp079BlackoutRoomAbility);

                case Scp079BlackoutZoneAbility scp079BlackoutZoneAbility:
                    return new Scp079ZoneBlackoutAbility(Player, scp079BlackoutZoneAbility);

                case Scp079DoorLockChanger scp079DoorLockChanger:
                    return new Scp079DoorLockAbility(Player, scp079DoorLockChanger);

                case Scp079DoorLockReleaser scp079DoorLockReleaser:
                    return new AbilityWrapper<Scp079DoorLockReleaser>(Player, scp079DoorLockReleaser);

                case Scp079DoorStateChanger scp079DoorStateChanger:
                    return new AbilityWrapper<Scp079DoorStateChanger>(Player, scp079DoorStateChanger);

                case Scp079ElevatorStateChanger scp079ElevatorStateChanger:
                    return new Scp079ElevatorUseAbility(Player, scp079ElevatorStateChanger);

                case Scp079LockdownRoomAbility scp079LockdownRoomAbility:
                    return new Scp079RoomLockdownAbility(Player, scp079LockdownRoomAbility);

                case Scp079LostSignalHandler scp079LostSignalHandler:
                    return new Scp079Signal(Player, scp079LostSignalHandler);

                case Scp079ScannerTeamFilterSelector scp079ScannerTeamFilterSelector:
                    return new Scp079ScannerTeamFilter(Player, scp079ScannerTeamFilterSelector);

                case Scp079ScannerZoneSelector scp079ScannerZoneSelector:
                    return new Scp079ScannerZoneFilter(Player, scp079ScannerZoneSelector);

                case PlayerRoles.PlayableScps.Scp079.Scp079SpeakerAbility scp079SpeakerAbility:
                    return new Scp079SpeakerAbility(Player, scp079SpeakerAbility);

                case PlayerRoles.PlayableScps.Scp079.Scp079TeslaAbility scp079TeslaAbility:
                    return new Scp079TeslaAbility(Player, scp079TeslaAbility);

                case PlayerRoles.PlayableScps.Scp079.Pinging.Scp079PingAbility scp079PingAbility:
                    return new Scp079PingAbility(Player, scp079PingAbility);

                case Scp079TierManager scp079TierManager:
                    return new Scp079Tier(Player, scp079TierManager);

                case Scp079CameraRotationSync scp079CameraRotationSync:
                    return new AbilityWrapper<Scp079CameraRotationSync>(Player, scp079CameraRotationSync);

                case Scp079CurrentCameraSync scp079CurrentCameraSync:
                    return new Scp079CameraManager(Player, scp079CurrentCameraSync);

                case Scp079ForwardCameraSelector scp079ForwardCameraSelector:
                    return new AbilityWrapper<Scp079ForwardCameraSelector>(Player, scp079ForwardCameraSelector);

                case Scp079OverconCameraSelector scp079OverconCameraSelector:
                    return new AbilityWrapper<Scp079OverconCameraSelector>(Player, scp079OverconCameraSelector);

                case Scp079DirectionalCameraSelector scp079DirectionalCameraSelector:
                    return new AbilityWrapper<Scp079DirectionalCameraSelector>(Player, scp079DirectionalCameraSelector);

                case Scp079MapToggler scp079MapToggler:
                    return new Scp079MapAbility(Player, scp079MapToggler);
                #endregion

                #region SCP-096
                case PlayerRoles.PlayableScps.Scp096.Scp096AttackAbility scp096AttackAbility:
                    return new Scp096AttackAbility(Player, scp096AttackAbility);

                case PlayerRoles.PlayableScps.Scp096.Scp096AudioPlayer scp096AudioPlayer:
                    return new Scp096AudioPlayer(Player, scp096AudioPlayer);

                case PlayerRoles.PlayableScps.Scp096.Scp096ChargeAbility scp096ChargeAbility:
                    return new Scp096ChargeAbility(Player, scp096ChargeAbility);

                case PlayerRoles.PlayableScps.Scp096.Scp096PrygateAbility scp096PryGateAbility:
                    return new Scp096PryGateAbility(Player, scp096PryGateAbility);

                case PlayerRoles.PlayableScps.Scp096.Scp096TryNotToCryAbility scp096TryNotToCryAbility:
                    return new Scp096TryNotToCryAbility(Player, scp096TryNotToCryAbility);

                case PlayerRoles.PlayableScps.Scp096.Scp096RageCycleAbility scpa096RageCycleAbility:
                    return new AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096RageCycleAbility>(Player, scpa096RageCycleAbility);

                case PlayerRoles.PlayableScps.Scp096.Scp096RageManager scp096RageManager:
                    return new AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096RageManager>(Player, scp096RageManager);

                case PlayerRoles.PlayableScps.Scp096.Scp096TargetsTracker scp096TargetsTracker:
                    return new AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096TargetsTracker>(Player, scp096TargetsTracker);
                #endregion

                #region SCP-106
                case PlayerRoles.PlayableScps.Scp106.Scp106Attack scp106Attack:
                    return new Scp106AttackAbility(Player, scp106Attack);

                case PlayerRoles.PlayableScps.Scp106.Scp106HuntersAtlasAbility scp106HuntersAtlasAbility:
                    return new Scp106HuntersAtlasAbility(Player, scp106HuntersAtlasAbility);

                case PlayerRoles.PlayableScps.Scp106.Scp106StalkAbility scp106StalkAbility:
                    return new Scp106StalkAbility(Player, scp106StalkAbility);

                case PlayerRoles.PlayableScps.Scp106.Scp106SinkholeController scp106SinkholeController:
                    return new Scp106Sinkhole(Player, scp106SinkholeController);
                #endregion

                #region SCP-173
                case PlayerRoles.PlayableScps.Scp173.Scp173AudioPlayer scp173AudioPlayer:
                    return new Scp173AudioPlayer(Player, scp173AudioPlayer);

                case PlayerRoles.PlayableScps.Scp173.Scp173BreakneckSpeedsAbility scp173BreakneckSpeedsAbility:
                    return new Scp173BreakneckSpeedsAbility(Player, scp173BreakneckSpeedsAbility);

                case PlayerRoles.PlayableScps.Scp173.Scp173SnapAbility scp173SnapAbility:
                    return new Scp173SnapAbility(Player, scp173SnapAbility);

                case PlayerRoles.PlayableScps.Scp173.Scp173TantrumAbility scp173TantrumAbility:
                    return new Scp173TantrumAbility(Player, scp173TantrumAbility);

                case PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility scp173TeleportAbility:
                    return new Scp173TeleportAbility(Player, scp173TeleportAbility);

                case PlayerRoles.PlayableScps.Scp173.Scp173BlinkTimer scp173BlinkTimer:
                    return new Scp173Blink(Player, scp173BlinkTimer);
                #endregion

                default:
                    Log.Warn($"Unknown base-game subroutine! {subroutineBase.GetType().FullName}");
                    return null;
            }
        }
    }
}