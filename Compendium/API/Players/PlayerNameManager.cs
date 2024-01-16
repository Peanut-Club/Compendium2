using Compendium.API.Nicknames;

namespace Compendium.API.Players
{
    public class PlayerNameManager : INicknameManager
    {
        public PlayerNameManager(Player owner)
            => Owner = owner;

        public IPlayer Owner { get; }

        public string Name
        {
            get => Owner.Base.nicknameSync.Network_myNickSync;
            set => Owner.Base.nicknameSync.Network_myNickSync = value;
        }

        public string DisplayName
        {
            get => Owner.Base.nicknameSync.Network_displayName;
            set => Owner.Base.nicknameSync.Network_displayName = value;
        }
    }
}