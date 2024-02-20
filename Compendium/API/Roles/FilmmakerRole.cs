using Common.Values;

namespace Compendium.API.Roles
{
    public class FilmmakerRole : Role, IWrapper<PlayerRoles.Filmmaker.FilmmakerRole>
    {
        public FilmmakerRole(PlayerRoles.Filmmaker.FilmmakerRole roleBase) : base(roleBase)
        {
            Base = roleBase;
        }

        public new PlayerRoles.Filmmaker.FilmmakerRole Base { get; }
    }
}