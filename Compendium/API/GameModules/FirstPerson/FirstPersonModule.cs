using Common.Values;

using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;

using RelativePositioning;

using UnityEngine;

namespace Compendium.API.GameModules.FirstPerson
{
    public class FirstPersonModule : IWrapper<FirstPersonMovementModule>
    {
        public FirstPersonModule(FirstPersonMovementModule firstPersonMovementModule)
        {
            Base = firstPersonMovementModule;

            Camera = new FirstPersonCamera(firstPersonMovementModule.MouseLook, this);
            NoClip = new FirstPersonNoClip(firstPersonMovementModule.Noclip, this);
            Motor = new FirstPersonMotor(firstPersonMovementModule.Motor, this);
        }

        public FirstPersonMovementModule Base { get; }
        public FirstPersonCamera Camera { get; }
        public FirstPersonNoClip NoClip { get; }
        public FirstPersonMotor Motor { get; }

        public CharacterModel Model
        {
            get => Base.CharacterModelInstance;
        }

        public CharacterController Controller
        {
            get => Base.CharController;
        }

        public CharacterControllerSettingsPreset Settings
        {
            get => Base.CharacterControllerSettings;
        }

        public Vector3 Position
        {
            get => Base.Position;
            set => Base.ServerOverridePosition(value, Vector3.zero);
        }

        public Vector3 Center
        {
            get => Settings.Center;
        }

        public RelativePosition RelativePosition
        {
            get => Base.Motor.ReceivedPosition;
            set => Position = value.Position;
        }

        public Quaternion Rotation
        {
            get => Camera.Rotation;
            set => Camera.Rotation = value;
        }

        public float Height
        {
            get => Settings.Height;
        }

        public float Radius
        {
            get => Settings.Radius;
        }

        public float Width
        {
            get => Settings.SkinWidth;
        }

        public bool IsGrounded
        {
            get => Base.IsGrounded;
        }

        public bool IsMoving
        {
            get => Base.Motor.MovementDetected;
        }
    }
}