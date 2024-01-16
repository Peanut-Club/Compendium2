using UnityEngine;

namespace Compendium.Map
{
    public interface IFacilityElement
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
    }
}