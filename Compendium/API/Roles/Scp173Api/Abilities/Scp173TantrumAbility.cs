using Compendium.API.Roles.Abilities;

using Mirror;

using RelativePositioning;

using UnityEngine;

namespace Compendium.API.Roles.Scp173Api.Abilities
{
    public class Scp173TantrumAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp173.Scp173TantrumAbility>
    {
        public Scp173TantrumAbility(Player player, PlayerRoles.PlayableScps.Scp173.Scp173TantrumAbility ability) : base(player, ability)
        {
            Cooldown = 30f;
        }

        public float Cooldown { get; set; }

        public float RemainingCooldown
        {
            get => Base.Cooldown.Remaining;
            set
            {
                Base.Cooldown.Remaining = value;
                Base.ServerSendRpc(true);
            }
        }

        public void Spawn(Vector3 position, Vector3 scale, Quaternion rotation, bool useCooldown = true)
        {
            var hazard = Object.Instantiate(Base._tantrumPrefab);

            hazard.SynchronizedPosition = new RelativePosition(position);

            hazard.transform.localScale = scale;
            hazard.transform.rotation = rotation;

            NetworkServer.Spawn(hazard.gameObject);

            foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
            {
                if (tesla.IsInIdleRange(Player.Base))
                    tesla.TantrumsToBeDestroyed.Add(hazard);
            }

            if (useCooldown)
            {
                Base.Cooldown.Trigger(Cooldown);
                Base.ServerSendRpc(true);
            }
        }
    }
}