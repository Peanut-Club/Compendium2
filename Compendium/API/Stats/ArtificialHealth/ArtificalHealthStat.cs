using Common.Pooling.Pools;

using PlayerStatsSystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Compendium.API.Stats.ArtificialHealth
{
    public class ArtificialHealthStat : Stat<AhpStat>
    {
        public ArtificialHealthStat(Player player, AhpStat stat) : base(player, stat) { }

        public IReadOnlyList<ArtificialHealthProcess> Processes { get; private set; }

        public override float MaxValue
        {
            get => Base._maxSoFar;
            set => Base._maxSoFar = value;
        }

        public ArtificialHealthProcess GetProcess(int processCode)
            => Processes.FirstOrDefault(p => p.Code == processCode);

        public ArtificialHealthProcess GetProcess(AhpStat.AhpProcess ahpProcess)
            => Processes.FirstOrDefault(p => p.Code == ahpProcess.KillCode || p.Base == ahpProcess);

        public ArtificialHealthProcess AddProcess(float amount, float limit = 75f, float decay = 1.2f, float efficacy = 0.7f, float sustain = 0f, bool isPersistent = false)
        {
            var process = new ArtificialHealthProcess(amount, limit, decay, efficacy, sustain, isPersistent);

            AddNewProcess(process.Base);

            process.OnAdded();

            return process;
        }

        public bool RemoveProcess(int processCode)
            => Base.ServerKillProcess(processCode);

        internal void AddNewProcess(AhpStat.AhpProcess ahpProcess)
        {
            var num = 0f;
            var num2 = ahpProcess.Limit;

            foreach (var curAhpProcess in Base._activeProcesses)
            {
                num += curAhpProcess.CurrentAmount;
                num2 = Mathf.Max(num2, curAhpProcess.Limit);
            }

            var num3 = num + ahpProcess.CurrentAmount - num2;

            if (num3 > 0f)
                ahpProcess.CurrentAmount = Mathf.Max(0f, ahpProcess.CurrentAmount - num3);

            for (int i = 0; i < Base._activeProcesses.Count; i++)
            {
                if (ahpProcess.Efficacy >= Base._activeProcesses[i].Efficacy)
                {
                    Base._activeProcesses.Insert(i, ahpProcess);
                    return;
                }
            }

            Base._activeProcesses.Add(ahpProcess);
        }

        internal void AddProcess(AhpStat.AhpProcess ahpProcess)
        {
            if (Processes is null)
            {
                Processes = new List<ArtificialHealthProcess>(new ArtificialHealthProcess[] { new ArtificialHealthProcess(ahpProcess) });
                return;
            }

            var process = new ArtificialHealthProcess(ahpProcess);
            var processes = ListPool<ArtificialHealthProcess>.Shared.Rent(Processes);

            processes.Add(process);

            Processes = processes.AsReadOnly();

            ListPool<ArtificialHealthProcess>.Shared.Return(processes);

            process.OnAdded();
        }

        internal void RemoveProcess(AhpStat.AhpProcess ahpProcess)
        {
            if (Processes is null)
                return;

            var processes = ListPool<ArtificialHealthProcess>.Shared.Rent(Processes);
            var processIndex = processes.FindIndex(p => p.Code == ahpProcess.KillCode);

            if (processIndex != -1)
            {
                processes[processIndex].OnRemoved();
                processes.RemoveAt(processIndex);
            }

            Processes = processes.AsReadOnly();

            ListPool<ArtificialHealthProcess>.Shared.Return(processes);
        }
    }
}