using Compendium.API.Roles.Abilities;

using UnityEngine;

namespace Compendium.API.Roles.Scp096Api.Abilities
{
    public class Scp096TryNotToCryAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp096.Scp096TryNotToCryAbility>
    {
        public Scp096TryNotToCryAbility(Player player, PlayerRoles.PlayableScps.Scp096.Scp096TryNotToCryAbility ability) : base(player, ability) { }

        public bool IsActive
        {
            get => Base.IsActive;
            set => Base.IsActive = value;
        }

        public bool IsMovementLocked
        {
            get => Base.LockMovement;
        }

        public Vector3 Position
        {
            get => Base._syncPoint.Position;
            set
            {
                Base._syncPoint = new RelativePositioning.RelativePosition(value);
                Base.ServerSendRpc(true);
            }
        }

        public Quaternion Rotation
        {
            get => Base._syncRot;
            set
            {
                Base._syncRot = value;
                Base.ServerSendRpc(true);
            }
        }

        public void Start()
        {
            Base._syncPoint = new RelativePositioning.RelativePosition(Player.Base);
            Base._syncRot = Player.Rotation;
            Base.IsActive = true;
            Base.ServerSendRpc(true);
        }
    }
}