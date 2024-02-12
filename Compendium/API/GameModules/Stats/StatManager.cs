using Common.Pooling.Pools;
using Common.Values;

using Compendium.API.Modules;
using Compendium.API.Stats.ArtificialHealth;
using Compendium.API.Stats;

using System.Collections.Generic;
using System;

using PlayerStatsSystem;

using AdminFlagsStat = Compendium.API.Stats.AdminFlagsStat;

namespace Compendium.API.GameModules.Stats
{
    public class StatManager : ModuleBase, IWrapper<PlayerStats>
    {       
        public PlayerStats Base { get; private set; }
        public Player Player { get; private set; }

        public AdminFlagsStat AdminFlags { get; private set; }
        public ArtificialHealthStat ArtificialHealth { get; private set; }

        public Stat<HealthStat> Health { get; private set; }
        public Stat<StaminaStat> Stamina { get; private set; }
        public Stat<HumeShieldStat> HumeShield { get; private set; }

        public IReadOnlyList<IStat> Stats { get; private set; }

        public override ModuleUpdate OnStart()
        {
            if (Manager is not API.Player)
                throw new InvalidOperationException($"This module can only be assigned to a Player!");

            Player = (Player)Manager;
            Base = Player.Base.playerStats;

            RefreshStats();

            return null;
        }

        public void RefreshStats()
        {
            var stats = ListPool<IStat>.Shared.Rent();

            for (int i = 0; i < Base.StatModules.Length; i++)
            {
                var stat = GetStat(Base.StatModules[i]);

                if (stat != null)
                    stats.Add(stat);
            }

            Stats = stats.AsReadOnly();

            ListPool<IStat>.Shared.Return(stats);
        }

        public TStat Get<TStat>(StatType statType) where TStat : IStat
            => (TStat)Get(statType);

        public IStat Get(StatType statType)
        {
            switch (statType)
            {
                case StatType.AdminFlags:
                    return AdminFlags;

                case StatType.ArtificialHealth:
                    return ArtificialHealth;

                case StatType.Health:
                    return Health;

                case StatType.HumeShield:
                    return HumeShield;

                case StatType.Stamina:
                    return Stamina;

                default:
                    Log.Warn($"Undefined stat type: {statType}");
                    return null;
            }
        }

        private IStat GetStat(StatBase statBase)
        {
            switch (statBase)
            {
                case PlayerStatsSystem.AdminFlagsStat adminFlagsStat:
                    return AdminFlags = new AdminFlagsStat(Player, adminFlagsStat);

                case AhpStat ahpStat:
                    return ArtificialHealth = new ArtificialHealthStat(Player, ahpStat);

                case HealthStat healthStat:
                    return Health = new Stat<HealthStat>(Player, healthStat);

                case HumeShieldStat humeShieldStat:
                    return HumeShield = new Stat<HumeShieldStat>(Player, humeShieldStat);

                case StaminaStat staminaStat:
                    return Stamina = new Stat<StaminaStat>(Player, staminaStat);

                default:
                    Log.Warn($"Undefined base-game stat: {statBase.GetType().FullName}");
                    return null;
            }
        }
    }
}