using Compendium.API.Roles.Abilities;

using UnityEngine;

namespace Compendium.API.Roles.Scp173Api.Abilities
{
    public class Scp173TeleportAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility>
    {
        public Scp173TeleportAbility(Player player, PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility ability) : base(player, ability) { }

        public bool IsAiming
        {
            get => Base.HasDataFlag(PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility.CmdTeleportData.Aiming);
            set
            {
                Base._cmdData = value ? Base._cmdData | PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility.CmdTeleportData.Aiming : Base._cmdData & ~PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility.CmdTeleportData.Aiming;
                Base.ServerSendRpc(true);
            }
        }

        public bool WantsToTeleport
        {
            get => Base.HasDataFlag(PlayerRoles.PlayableScps.Scp173.Scp173TeleportAbility.CmdTeleportData.WantsToTeleport);
        }

        public Vector3 Position
        {
            get => Base._tpPosition;
        }

        public Player Target
        {
            get => Player.Get(Base.BestTarget);
        }

        public void Blink(Vector3 teleportPosition)
        {
            Base._blinkTimer.ServerBlink(teleportPosition + Vector3.up * (Base._fpcModule.CharController.height / 2f));
        }
    }
}