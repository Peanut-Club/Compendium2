using Compendium.API.Enums;
using Compendium.API.Roles.Abilities;

using UnityEngine;

using Mirror;

using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp3114;

using PlayerStatsSystem;

namespace Compendium.API.Roles.Scp3114Api.Abilites
{
    public class Scp3114DisguiseAbility : AbilityWrapper<Scp3114Disguise>
    {
        private Scp3114Identity identity;

        public Scp3114DisguiseAbility(Player player, Scp3114Disguise ability) : base(player, ability)
        {
            ability.CastRole.SubroutineModule.TryGetSubroutine(out identity);
        }

        public RoleTypeId Role
        {
            get => Status is Scp3114DisguiseStatus.Active ? identity.CurIdentity.StolenRole : RoleTypeId.None;
            set
            {
                if (value is RoleTypeId.None)
                {
                    Status = Scp3114DisguiseStatus.None;
                    return;
                }

                if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(value, out var role) || role is not FpcStandardRoleBase fpcRole || fpcRole.Ragdoll is null)
                    return;

                var ragdoll = Object.Instantiate(fpcRole.Ragdoll);

                ragdoll.NetworkInfo = new RagdollData(Player.Base, new UniversalDamageHandler(-1f, DeathTranslations.Unknown), value, Vector3.zero, Quaternion.identity, Player.Name, NetworkTime.time);
                ragdoll.transform.position = Vector3.zero;
                ragdoll.transform.rotation = Quaternion.identity;

                NetworkServer.Spawn(ragdoll.gameObject);

                identity.CurIdentity.Reset();
                identity.CurIdentity.Ragdoll = ragdoll;
                identity.CurIdentity.UnitNameId = 0;
                identity.CurIdentity.Status = Scp3114Identity.DisguiseStatus.Active;

                identity.ServerSendRpc(true);
            }
        }

        public Scp3114DisguiseStatus Status
        {
            get => (Scp3114DisguiseStatus)identity.CurIdentity.Status;
            set
            {
                identity.CurIdentity.Status = (Scp3114Identity.DisguiseStatus)value;
                identity.ServerSendRpc(true);
            }
        }

        public byte UnitId
        {
            get => identity.CurIdentity.UnitNameId;
            set
            {
                identity.CurIdentity.UnitNameId = value;
                identity.ServerSendRpc(true);
            }
        }

        public bool IsDisguised
        {
            get => Status is Scp3114DisguiseStatus.Active;
        }

        public bool IsDisguising
        {
            get => Status is Scp3114DisguiseStatus.Equipping;
        }

        public bool WasDisguised
        {
            get => identity._wasDisguised;
        }

        public void Cancel()
            => Status = Scp3114DisguiseStatus.None;

        public void Disguise(RoleTypeId role)
            => Role = role;
    }
}