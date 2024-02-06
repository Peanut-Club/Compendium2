using Common.Pooling.Pools;
using Common.Values;

using Compendium.API.Extensions;
using Compendium.API.Utilities;

using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps;

using System;

using UnityEngine;

namespace Compendium.API.GameModules.FirstPerson
{
    public class FirstPersonCamera : IWrapper<FpcMouseLook>
    {
        private static int playerMaskValue = -1;

        public static int PlayerMask
        {
            get
            {
                if (playerMaskValue < 0)
                {
                    playerMaskValue = 0;

                    var layer = LayerMask.NameToLayer("Player");

                    for (int i = 0; i < 32; i++)
                    {
                        if (!Physics.GetIgnoreLayerCollision(layer, i))
                            playerMaskValue |= 1 << i;
                    }
                }

                return playerMaskValue;
            }
        }

        public FirstPersonCamera(FpcMouseLook mouseLook, FirstPersonModule firstPersonModule)
        {
            Base = mouseLook;
            Module = firstPersonModule;
        }

        public FpcMouseLook Base { get; }
        public FirstPersonModule Module { get; }

        public Transform Camera
        {
            get => Base._hub.PlayerCameraReference;
        }

        public Quaternion Rotation
        {
            get => Base._hub.transform.rotation;
            set => Base._fpmm.ServerOverridePosition(Base._fpmm.Position, value.eulerAngles);
        }

        public Vector3 VectorRotation
        {
            get => Base._hub.transform.rotation.eulerAngles;
            set => Base._fpmm.ServerOverridePosition(Base._fpmm.Position, value);
        }

        public ClientRotation ClientRotation
        {
            get => new ClientRotation(Rotation);
            set => Base._fpmm.ServerOverridePosition(Base._fpmm.Position, new Vector3(value.HorizontalAxis, value.VerticalAxis));
        }

        public bool IsMoving
        {
            get => Base._fpmm.Motor.RotationDetected;
        }

        public Player PlayerInSight
        {
            get
            {
                foreach (var player in Player.List)
                {
                    if (player.Role.IsAlive && IsLookingAt(player))
                        return player;
                }

                return null;
            }
        }

        public Player[] PlayersInSight
        {
            get
            {
                var list = ListPool<Player>.Shared.Rent();

                foreach (var player in Player.List)
                {
                    if (player.Role.IsAlive && IsInLineOfSight(player))
                        list.Add(player);
                }

                return ListPool<Player>.Shared.ToArrayReturn(list);
            }
        }

        public bool IsInLineOfSight(Vector3 target)
            => GetVisionOf(target, 0).IsInLineOfSight;

        public bool IsInLineOfSight(Player target)
            => GetVisionOf(target.Position, PlayerMask).IsInLineOfSight;

        public bool IsInLineOfSight(ReferenceHub target)
            => GetVisionOf(target.GetRealPosition(), PlayerMask).IsInLineOfSight;

        public bool IsInLineOfSight(GameObject target)
            => GetVisionOf(target.transform.position, 0).IsInLineOfSight;

        public bool IsInLineOfSight(Transform target)
            => GetVisionOf(target.position, 0).IsInLineOfSight;

        public bool IsLookingAt(Vector3 target)
            => GetVisionOf(target, 0).IsLooking;

        public bool IsLookingAt(Player target)
            => GetVisionOf(target.Position, PlayerMask).IsLooking;

        public bool IsLookingAt(ReferenceHub target)
            => GetVisionOf(target.GetRealPosition(), PlayerMask).IsLooking;

        public bool IsLookingAt(GameObject target)
            => GetVisionOf(target.transform.position, 0).IsLooking;

        public bool IsLookingAt(Transform target)
            => GetVisionOf(target.position, 0).IsLooking;

        public VisionInformation GetVisionOf(Vector3 target, int mask, float radius = 0f, float trigger = 0f, bool checkFog = true, bool checkLineOfSight = true, bool checkDarkness = true)
            => VisionInformation.GetVisionInformation(Base._hub, Camera, target, radius, trigger, checkFog, checkLineOfSight, mask, checkDarkness);

        public RaycastHit CastForward(float distance, int mask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
            => RaycastUtils.CastForwardHit(Base._hub, distance, mask, queryTriggerInteraction);

        public RaycastHit[] CastForward(float distance, int mask, int hits, QueryTriggerInteraction queryTriggerInteraction, out Action onFinished)
            => RaycastUtils.CastDownwardsHits(Base._hub, distance, mask, hits, out onFinished, queryTriggerInteraction);

        public RaycastHit CastDownward(float distance, int mask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
            => RaycastUtils.CastDownwardsHit(Base._hub, distance, mask, queryTriggerInteraction);

        public RaycastHit[] CastDownward(float distance, int mask, int hits, QueryTriggerInteraction queryTriggerInteraction, out Action onFinished)
            => RaycastUtils.CastDownwardsHits(Base._hub, distance, mask, hits, out onFinished, queryTriggerInteraction);
    }
}