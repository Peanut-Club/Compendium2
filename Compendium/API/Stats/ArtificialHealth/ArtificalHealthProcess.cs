using Compendium.API.Core;

using PlayerStatsSystem;

namespace Compendium.API.Stats.ArtificialHealth
{
    public class ArtificialHealthProcess : Wrapper<AhpStat.AhpProcess>
    {
        public float Amount
        {
            get => Base.CurrentAmount;
            set => Base.CurrentAmount = value;
        }

        public float Limit
        {
            get => Base.Limit;
            set => Base.Limit = value;
        }
        public float Decay
        {
            get => Base.DecayRate;
            set => Base.DecayRate = value;
        }
        public float Efficacy
        {
            get => Base.Efficacy;
            set => Base.Efficacy = value;
        }
        public float Sustain
        {
            get => Base.SustainTime;
            set => Base.SustainTime = value;
        }

        public int Code
        {
            get => Base.KillCode;
        }

        public bool IsPersistent
        {
            get => Base.Persistant;
        }

        public ArtificialHealthProcess(float amount, float limit, float decay, float efficacy, float sustain, bool isPersistent)
            : base(new AhpStat.AhpProcess(amount, limit, decay, efficacy, sustain, isPersistent)) { }

        public ArtificialHealthProcess(AhpStat.AhpProcess ahpProcess) 
            : base(ahpProcess) { }

        public virtual void OnAdded() { }
        public virtual void OnRemoved() { }
        public virtual void OnProgressed() { }
    }
}