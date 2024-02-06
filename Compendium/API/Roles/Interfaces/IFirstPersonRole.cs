using Compendium.API.GameModules.FirstPerson;

namespace Compendium.API.Roles.Interfaces
{
    public interface IFirstPersonRole : ISpawnPointRole
    {
        FirstPersonModule Module { get; }
    }
}