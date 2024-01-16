using Compendium.API.Utilities;

using UnityEngine;

namespace Compendium.API.Extensions
{
    public static class QuaternionExtensions
    {
        public static ClientRotation ToClientRotation(this Quaternion quaternion)
            => new ClientRotation(quaternion);

        public static Vector3 ToClientRotationVector(this Quaternion quaternion)
            => new Vector3(0f, new ClientRotation(quaternion).VerticalAxis);
    }
}