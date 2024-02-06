using Common.Pooling.Pools;

using Compendium.API.Roles.Abilities;

namespace Compendium.API.Roles.Scp049.Abilities
{
    public class Scp049ResurrectAbility : Ability<PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility>
    {
        public Scp049ResurrectAbility(Player player, PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility scp049ResurrectAbility) : base(player, scp049ResurrectAbility) { }

        public static int GetResurrections(Player player)
            => PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.GetResurrectionsNumber(player.Base);

        public static void ClearResurrections(Player player)
            => PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.ClearPlayerResurrections(player.Base);

        public static Player[] GetDeadZombies()
        {
            var players = ListPool<Player>.Shared.Rent();

            foreach (var networkId in PlayerRoles.PlayableScps.Scp049.Scp049ResurrectAbility.DeadZombies)
            {
                var player = Player.Get(networkId);

                if (player != null)
                    players.Add(player);
            }

            return ListPool<Player>.Shared.ToArrayReturn(players);
        }
    }
}