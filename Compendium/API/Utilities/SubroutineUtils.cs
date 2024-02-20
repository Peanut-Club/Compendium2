using PlayerRoles.Subroutines;
using PlayerRoles;

using System.Collections.Generic;
using System;

using MEC;

using Common.Logging;

using Mirror;

using Utils.Networking;

namespace Compendium.API.Utilities
{
    public static class SubroutineUtils
    {
        public static LogOutput Log { get; } = new LogOutput("Subroutine Utils").Setup();

        public static IReadOnlyDictionary<Type, int> SubroutineIndexes { get; private set; }

        public static IReadOnlyList<RoleTypeId> RolesWithSubroutines { get; } = new List<RoleTypeId>
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp0492,
            RoleTypeId.Scp079,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp3114,
            RoleTypeId.Scp939,
        };

        public static bool ServerSendSync<TRoutine>(this ReferenceHub player, ReferenceHub routineOwner = null, RoleTypeId? role = null, Action<NetworkWriter> routineDataWriter = null) where TRoutine : SubroutineBase
        {
            if (routineDataWriter is null)
                return ServerSendSync<TRoutine>(player, routineOwner, null, role);

            var routineData = NetworkWriterPool.Get();

            try
            {
                routineDataWriter(routineData);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }

            return ServerSendSync<TRoutine>(player, routineOwner, routineData, role);
        }

        public static bool ServerSendSync<TRoutine>(this ReferenceHub player, ReferenceHub routineOwner = null, NetworkWriter routineData = null, RoleTypeId? role = null, int routineIndex = -1) where TRoutine : SubroutineBase
        {
            routineOwner ??= player;

            if (routineOwner is null)
                return false;

            if (routineIndex < 0)
                routineIndex = GetSyncIndex<TRoutine>();

            if (routineIndex < 0)
                return false;

            role ??= routineOwner.GetRoleId();

            using (var writer = NetworkWriterPool.Get())
            {
                writer.WriteUShort(NetworkMessageId<SubroutineMessage>.Id);
                writer.WriteByte((byte)routineIndex);
                writer.WriteReferenceHub(routineOwner);
                writer.WriteRoleType(role.Value);

                if (routineData != null)
                {
                    var writerPos = routineData.Position;

                    if (writerPos > 65790)
                        writerPos = 0;

                    writer.WriteByte((byte)Math.Min(writerPos, byte.MaxValue));

                    if (writerPos >= byte.MaxValue)
                        writer.WriteUShort((ushort)(writerPos - byte.MaxValue));

                    writer.WriteBytes(routineData.buffer, 0, writerPos);

                    if (routineData is NetworkWriterPooled pooled)
                        pooled.Dispose();
                }

                routineOwner.connectionToClient.Send(writer.ToArraySegment());
                return true;
            }
        }

        public static int GetSyncIndex<TRoutine>() where TRoutine : SubroutineBase
            => SubroutineIndexes.TryGetValue(typeof(TRoutine), out var routineIndex) ? routineIndex : -1;

        internal static void ReloadIndexes()
        {
            if (SubroutineIndexes != null)
                return;

            Log.Verbose($"Reloading subroutine indexes ..");

            Timing.RunCoroutine(ReloadIndexesRoutine());
        }

        private static IEnumerator<float> ReloadIndexesRoutine()
        {
            var syncDict = new Dictionary<Type, int>();

            for (int i = 0; i < RolesWithSubroutines.Count; i++)
            {
                try
                {
                    ReferenceHub.HostHub.roleManager.ServerSetRole(RolesWithSubroutines[i], RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                }
                catch { continue; }

                yield return Timing.WaitForSeconds(0.3f);

                if (ReferenceHub.HostHub.roleManager.CurrentRole is null || ReferenceHub.HostHub.roleManager.CurrentRole.RoleTypeId != RolesWithSubroutines[i]
                    || ReferenceHub.HostHub.roleManager.CurrentRole is not ISubroutinedRole subroutinedRole || subroutinedRole.SubroutineModule is null)
                {
                    Log.Error($"Invalid subroutined role: {RolesWithSubroutines[i]}");
                    continue;
                }

                yield return Timing.WaitForSeconds(0.2f);

                for (int x = 0; x < subroutinedRole.SubroutineModule.AllSubroutines.Length; x++)
                {
                    var routine = subroutinedRole.SubroutineModule.AllSubroutines[x];
                    var routineType = routine.GetType();

                    syncDict[routineType] = routine.SyncIndex;

                    Log.Verbose($"Cached sync index of routine '{routineType.FullName}': {syncDict[routineType]} ({routine.SyncIndex})");
                }
            }

            SubroutineIndexes = syncDict;

            Log.Verbose($"Cached {SubroutineIndexes.Count} subroutine indexes.");

            try
            {
                ReferenceHub.HostHub.roleManager.ServerSetRole(RoleTypeId.None, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            }
            catch { }
        }
    }
}