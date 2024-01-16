using PlayerRoles.FirstPersonControl;

using UnityEngine;

namespace Compendium.API.Utilities
{
    public static class RaycastUtils
    {
        public static Vector3 GetPlayerCastPosition(this ReferenceHub hub)
        {
            var hubPos = hub.transform.position;
            hubPos.x += 0.15f;
            return hubPos;
        }

        public static RaycastHit CastDownwardsGround(Vector3 position, float distance)
            => Physics.Raycast(position, Vector3.down, out var hit, distance, FpcStateProcessor.Mask, QueryTriggerInteraction.Ignore) ? hit : default;
    }
}