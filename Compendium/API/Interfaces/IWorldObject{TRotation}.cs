namespace Compendium.API.Interfaces
{
    public interface IWorldObject<TRotation> : IWorldObject
        where TRotation : struct
    { 
        TRotation Rotation { get; }
    }
}