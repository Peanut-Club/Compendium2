namespace Compendium.API.Effects
{
    public class EffectManager
    {
        public EffectManager(Player owner)
        {
            Owner = owner;
        }

        public Player Owner { get; }
    }
}