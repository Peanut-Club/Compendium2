using Common.Values;

using PlayerStatsSystem;

using UnityEngine;

namespace Compendium.API.Roles
{
    public class SpectatorRole : Role, IWrapper<PlayerRoles.Spectating.SpectatorRole>
    {
        public SpectatorRole(PlayerRoles.Spectating.SpectatorRole roleBase) : base(roleBase)
        {
            Base = roleBase;

            if (Base._damageHandler != null)
                DamageHandler = new DamageHandler((StandardDamageHandler)Base._damageHandler);
            else
                DamageHandler = DamageHandler.Get(Enums.DamageType.Unknown, 0f);
        }

        public new PlayerRoles.Spectating.SpectatorRole Base { get; }

        public DamageHandler DamageHandler { get; }

        public Player SpectatedPlayer
        {
            get => Player.Get(Base.SyncedSpectatedNetId);
        }

        public Vector3 DeathPosition
        {
            get => Base.DeathPosition.Position;
        }

        public bool CanRespawn
        {
            get => Base.ReadyToRespawn;
        }
    }
}