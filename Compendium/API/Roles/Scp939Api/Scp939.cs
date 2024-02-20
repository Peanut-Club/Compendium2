using Common.Values;

using Compendium.API.Roles.Other;

using Mirror;

using PlayerRoles.PlayableScps.Scp939;

using UnityEngine;

using RelativePositioning;

namespace Compendium.API.Roles.Scp939Api
{
    public class Scp939 : SubroutinedRole, IWrapper<Scp939Role>
    {
        private Scp939AmnesticCloudAbility amnesticCloudAbility;
        private Scp939LungeAbility lungeAbility;
        private Scp939FocusAbility focusAbility;

        public Scp939(Scp939Role scpRole) : base(scpRole)
        {
            Base = scpRole;

            Base.SubroutineModule.TryGetSubroutine(out amnesticCloudAbility);
            Base.SubroutineModule.TryGetSubroutine(out focusAbility);
        }

        public new Scp939Role Base { get; }

        public bool IsFocused
        {
            get => focusAbility.TargetState;
            set
            {
                focusAbility.TargetState = value;
                focusAbility.ServerSendRpc(true);
            }
        }

        public Enums.Scp939LungeState LungeState
        {
            get => (Enums.Scp939LungeState)lungeAbility.State;
            set
            {
                lungeAbility.State = (Scp939LungeState)value;
                lungeAbility.ServerSendRpc(true);
            }
        }

        public Player LungeTarget
        {
            get => Player.Get(lungeAbility._playerToHit?._lastOwner);
        }

        public Scp939AmnesticCloudInstance SpawnAmnesticCloud(Vector3 position)
        {
            var instance = Object.Instantiate(amnesticCloudAbility._instancePrefab);

            instance.ServerSetup(Player.Base);
            instance.transform.position = position;

            NetworkServer.Spawn(instance.gameObject);

            instance.Network_syncPos = new RelativePosition(position);

            return instance;
        }
    }
}