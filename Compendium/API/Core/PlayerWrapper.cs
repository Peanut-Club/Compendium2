namespace Compendium.API.Core
{
    public class PlayerWrapper<T> : Wrapper<T>
    {
        public PlayerWrapper(Player player, T baseValue) : base (baseValue)
        {
            Player = player;
        }

        public Player Player { get; }
    }
}