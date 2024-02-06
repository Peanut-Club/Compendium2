namespace Compendium.API.Stats.Admin
{
    public class AdminFlagsStat : Stat<PlayerStatsSystem.AdminFlagsStat>
    {
        public AdminFlagsStat(Player player, PlayerStatsSystem.AdminFlagsStat stat) : base(player, stat) { }

        public AdminFlags Flags
        {
            get => (AdminFlags)Base.Flags;
            set => Base.Flags = (PlayerStatsSystem.AdminFlags)value;
        }

        public bool IsNoClip
        {
            get => Has(AdminFlags.Noclip);
            set => Set(AdminFlags.Noclip, value);
        }

        public bool IsGodMode
        {
            get => Has(AdminFlags.GodMode);
            set => Set(AdminFlags.GodMode, value);
        }

        public bool IsBypassMode
        {
            get => Has(AdminFlags.BypassMode);
            set => Set(AdminFlags.BypassMode, value);
        }

        public bool Has(AdminFlags flag)
            => Base.HasFlag((PlayerStatsSystem.AdminFlags)flag);

        public void Set(AdminFlags flag, bool value)
            => Base.SetFlag((PlayerStatsSystem.AdminFlags)flag, value);

        public void Invert(AdminFlags flag)
            => Base.InvertFlag((PlayerStatsSystem.AdminFlags)flag);
    }
}