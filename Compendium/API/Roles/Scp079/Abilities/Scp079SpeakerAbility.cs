using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp079.Abilities
{
    public class Scp079SpeakerAbility : AbilityWrapper<PlayerRoles.PlayableScps.Scp079.Scp079SpeakerAbility>
    {
        public Scp079SpeakerAbility(Player player, PlayerRoles.PlayableScps.Scp079.Scp079SpeakerAbility ability) : base(player, ability) { }

        public bool CanTransmit
        {
            get => Base.CanTransmit;
        }

        public bool IsUsingSpeaker
        {
            get => Base._syncUsing;
            set
            {
                Base._syncUsing = value;
                Base.ServerSendRpc(true);
            }
        }
    }
}