using PlayerRoles;

namespace Compendium.API.Extensions
{
    public static class TeamExtensions
    {
        public static bool IsEnemyTo(this Team team, Team other, bool chaosAsScps = false)
        {
            if (team == other || team is Team.OtherAlive || other is Team.OtherAlive || team is Team.Dead || other is Team.Dead)
                return false;

            switch (team)
            {
                case Team.SCPs:
                    return other is Team.SCPs || (other is Team.ChaosInsurgency && chaosAsScps);

                case Team.ClassD:
                    return other != Team.ChaosInsurgency;

                case Team.FoundationForces:
                    return other != Team.Scientists;

                case Team.ChaosInsurgency:
                    return other is Team.SCPs ? !chaosAsScps : other != Team.ClassD;
            }

            return false;
        }
    }
}