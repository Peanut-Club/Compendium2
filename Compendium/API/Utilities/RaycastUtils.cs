using Common.Pooling.Pools;

using Compendium.API.Extensions;

using UnityEngine;

using System;

namespace Compendium.API.Utilities
{
    public static class RaycastUtils
    {
        public static float GetModelHeight(this ReferenceHub hub)
        {
            if (!hub.TryGetFpmm(out var fpmm))
                return 0f;

            return fpmm.CharacterControllerSettings.Height;
        }

        public static Vector3 GetForwardCastPosition(this ReferenceHub hub)
        {
            var hubPos = hub.transform.position;

            hubPos.x += 0.15f;

            return hubPos;
        }

        public static RaycastHit CastForwardHit(this ReferenceHub hub, float distance, int mask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var hitsNonAlloc = ArrayPool<RaycastHit>.Shared.Rent(1);

            Physics.RaycastNonAlloc(
                hub.GetRealPosition(),
                hub.GetForwardCastPosition(),

                hitsNonAlloc,

                distance,
                mask,

                triggerInteraction);

            var hit = hitsNonAlloc[0];

            ArrayPool<RaycastHit>.Shared.Return(hitsNonAlloc);

            return hit;
        }

        public static RaycastHit[] CastForwardHits(this ReferenceHub hub, float distance, int mask, int hits, out Action onFinished, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            if (hits < 0)
                throw new ArgumentOutOfRangeException(nameof(hits));

            var hitsNonAlloc = ArrayPool<RaycastHit>.Shared.Rent(hits);

            Physics.RaycastNonAlloc(
                hub.GetRealPosition(),
                hub.GetForwardCastPosition(),

                hitsNonAlloc,

                distance,
                mask,

                triggerInteraction);

            onFinished = () => ArrayPool<RaycastHit>.Shared.Return(hitsNonAlloc);

            return hitsNonAlloc;
        }

        public static RaycastHit CastDownwardsHit(this ReferenceHub hub, float distance, int mask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var hitsNonAlloc = ArrayPool<RaycastHit>.Shared.Rent(1);
            var castPosition = hub.GetForwardCastPosition();

            castPosition.y -= hub.GetModelHeight();

            Physics.RaycastNonAlloc(
                hub.GetRealPosition(),

                castPosition,
                hitsNonAlloc,
                distance,
                mask,

                triggerInteraction);

            var hit = hitsNonAlloc[0];

            ArrayPool<RaycastHit>.Shared.Return(hitsNonAlloc);

            return hit;
        }

        public static RaycastHit[] CastDownwardsHits(this ReferenceHub hub, float distance, int mask, int hits, out Action onFinished, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
        {
            if (hits < 0)
                throw new ArgumentOutOfRangeException(nameof(hits));

            var hitsNonAlloc = ArrayPool<RaycastHit>.Shared.Rent(hits);
            var castPosition = hub.GetForwardCastPosition();

            castPosition.y -= hub.GetModelHeight();

            Physics.RaycastNonAlloc(
                hub.GetRealPosition(),

                castPosition,
                hitsNonAlloc,
                distance,
                mask,

                triggerInteraction);

            onFinished = () => ArrayPool<RaycastHit>.Shared.Return(hitsNonAlloc);

            return hitsNonAlloc;
        }
    }
}