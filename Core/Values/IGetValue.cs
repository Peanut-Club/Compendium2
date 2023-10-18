namespace Compendium.Values
{
    public interface IGetValue<TValue>
    {
        TValue Value { get; }
    }
}