using Common.Values;

using PlayerRoles.FirstPersonControl;

using UnityEngine;

namespace Compendium.API.GameModules.FirstPerson
{
    public class FirstPersonMotor : IWrapper<FpcMotor>
    {
        public FirstPersonMotor(FpcMotor fpcMotor, FirstPersonModule module)
        {
            Base = fpcMotor;
            CanReceiveFallDamage = fpcMotor._enableFallDamage;
            Module = module;
        }

        public FpcMotor Base { get; }
        public FirstPersonModule Module { get; }

        public Vector3 Direction
        {
            get => Base.MoveDirection;
        }

        public Vector3 Velocity
        {
            get => Base.Velocity;
            set => Base.Velocity = value;
        }

        public bool CanReceiveFallDamage { get; set; }

        public bool IsInputLocked
        {
            get => Base.InputLocked;
        }

        public bool IsJumping
        {
            get => Base.IsJumping;
        }

        public bool IsMoving
        {
            get => Base.MovementDetected;
        }

        public bool WantsToJump
        {
            get => Base.WantsToJump;
        }

        public static Vector3 Gravity
        {
            get => FpcMotor.Gravity;
        }

        public static Vector3 InvisiblePosition
        {
            get => FpcMotor.InvisiblePosition;
        }
    }
}