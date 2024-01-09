using PlayerRoles.FirstPersonControl;

using UnityEngine;

namespace Compendium.API.Extensions
{
    public static class ReferenceHubExtensions
    {
        public static Vector3 GetRealPosition(this ReferenceHub hub)
        {
            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is IFpcRole fpcRole
                && fpcRole.FpcModule != null)
                return fpcRole.FpcModule.Position;

            return hub.PlayerCameraReference.position;
        }

        public static Quaternion GetRealRotation(this ReferenceHub hub)
        {
            if (hub.roleManager.CurrentRole != null && hub.roleManager.CurrentRole is IFpcRole fpcRole
                && fpcRole.FpcModule != null && fpcRole.FpcModule.MouseLook != null)
                return fpcRole.FpcModule.MouseLook.TargetHubRotation;

            return hub.PlayerCameraReference.rotation;
        }
    }
}
