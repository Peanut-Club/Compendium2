using UnityEngine;

namespace Compendium.API.Extensions
{
    public static class QuaternionExtensions
    {
        public static (ushort horizontal, ushort vertical) ToClientUShorts(this Quaternion rotation)
        {
            if (rotation.eulerAngles.z != 0f)
                rotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);

            var outfHorizontal = rotation.eulerAngles.y;
            var outfVertical = -rotation.eulerAngles.x;

            if (outfVertical < -90f)
                outfVertical += 360f;
            else if (outfVertical > 270f)
                outfVertical -= 360f;

            return (ToHorizontal(outfHorizontal), ToVertical(outfVertical));

            static ushort ToHorizontal(float horizontal)
            {
                const float ToHorizontal = 65535f / 360f;
                horizontal = Mathf.Clamp(horizontal, 0f, 360f);
                return (ushort)Mathf.RoundToInt(horizontal * ToHorizontal);
            }

            static ushort ToVertical(float vertical)
            {
                const float ToVertical = 65535f / 176f;
                vertical = Mathf.Clamp(vertical, -88f, 88f) + 88f;
                return (ushort)Mathf.RoundToInt(vertical * ToVertical);
            }
        }
    }
}
