namespace Compendium.API.GameModules.Stats
{
    public interface IStat
    {
        Player Player { get; }

        float Value { get; set; }

        float MaxValue { get; set; }
        float MinValue { get; set; }

        float NormalizedValue { get; }
    }
}