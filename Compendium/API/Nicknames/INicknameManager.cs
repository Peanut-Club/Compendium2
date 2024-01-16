using Compendium.API.Players;

namespace Compendium.API.Nicknames
{
    public interface INicknameManager : IPlayerModule
    {
        string Name { get; set; }
        string DisplayName { get; set; }
    }
}