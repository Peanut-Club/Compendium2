using CentralAuth;

namespace Compendium.API.Players
{
    public class PlayerModifiers
    {
        public bool ShouldShowInPlayerList { get; set; } = true;
        public bool ShouldShowInSpectatorList { get; set; } = true;
        public bool ShouldShowInRemoteAdminList { get; set; } = true;

        public ClientInstanceMode? ForcedInstanceMode { get; set; }
    }
}