using Common.Values;

using Compendium.API.GameModules.Stats;

using PlayerStatsSystem;

namespace Compendium.API.Stats
{
    public class Stat<TStat> : IStat, IWrapper<TStat>
        where TStat : StatBase
    {
        public Stat(Player player, TStat stat)
        {
            Player = player;
            Base = stat;

            MaxValue = stat.MaxValue;
            MinValue = stat.MinValue;
        }

        public TStat Base { get; }
        public Player Player { get; }

        public virtual float Value
        {
            get => Base.CurValue;
            set => Base.CurValue = value;
        }

        public virtual float NormalizedValue
        {
            get => Base.NormalizedValue;
        }

        public virtual float MaxValue { get; set; }
        public virtual float MinValue { get; set; }
    }
}