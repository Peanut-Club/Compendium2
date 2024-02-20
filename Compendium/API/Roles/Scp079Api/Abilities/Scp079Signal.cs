using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp079;

namespace Compendium.API.Roles.Scp079Api.Abilities
{
    public class Scp079Signal : AbilityWrapper<Scp079LostSignalHandler>
    {
        public Scp079Signal(Player player, Scp079LostSignalHandler ability) : base(player, ability) { }

        public bool IsLost
        {
            get => Base.Lost;
        }

        public bool WasLost
        {
            get => Base._prevLost;
        }

        public double Remaining
        {
            get => Base._recoveryTime;
            set
            {
                Base._recoveryTime = value;
                Base.ServerSendRpc(true);
            }
        }
    }
}