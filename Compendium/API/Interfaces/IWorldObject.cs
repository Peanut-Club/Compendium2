using Compendium.API.Enums;

using UnityEngine;

namespace Compendium.API.Interfaces
{
    public interface IWorldObject
    {
        Vector3 Position { get; }

        WorldObjectType ObjectType { get; }
    }
}