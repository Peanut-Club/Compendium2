using Compendium.API.Roles.Abilities;

using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace Compendium.API.Roles.Scp0492.Abilities
{
    public class Scp0492BloodlustAbility : Ability<ZombieBloodlustAbility>
    {
        public Scp0492BloodlustAbility(Player player, ZombieBloodlustAbility ability) : base(player, ability) { }

        public bool IsLookingAtTarget
        {
            get => Base.LookingAtTarget;
            set => Base.LookingAtTarget = value;
        }

        public bool IsSimulatingStare
        {
            get => Base.SimulatedStare > 0f;
        }

        public bool HasAnyTargets
        {
            get => Base.AnyTargets(Player.Base, Player.Camera);
        }
    }
}