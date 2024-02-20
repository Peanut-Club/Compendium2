using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp3114;

using RelativePositioning;

namespace Compendium.API.Roles.Scp3114Api.Abilites
{
    public class Scp3114StrangleAbility : AbilityWrapper<Scp3114Strangle>
    {
        public Scp3114StrangleAbility(Player player, Scp3114Strangle ability) : base(player, ability) { }

        public Player Target
        {
            get => Base.SyncTarget.HasValue ? Player.Get(Base.SyncTarget.Value.Target) : null;
            set
            {
                if (value is null)
                {
                    Base.SyncTarget = null;
                    Base._rpcType = Scp3114Strangle.RpcType.TargetResync;
                    Base.ServerSendRpc(true);
                    return;
                }

                Base.SyncTarget = new Scp3114Strangle.StrangleTarget(value.Base, new RelativePosition(value.Position), new RelativePosition(Player.Position));
                Base._rpcType = Scp3114Strangle.RpcType.TargetResync;
                Base.ServerSendRpc(true);
            }
        }

        public void Stop()
            => Target = null;
    }
}