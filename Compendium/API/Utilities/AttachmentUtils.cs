using Common.Pooling.Pools;

using Compendium.API.Enums;

using InventorySystem.Items.Firearms;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Utilities
{
    public static class AttachmentUtils
    {
        public static AttachmentType[] GetEnabled(this Firearm firearm)
        {
            var enabled = ListPool<AttachmentType>.Shared.Rent();

            for (int i = 0; i < firearm.Attachments.Length; i++)
            {
                if (firearm.Attachments[i].IsEnabled)
                    enabled.Add((AttachmentType)firearm.Attachments[i].Name);
            }

            return ListPool<AttachmentType>.Shared.ToArrayReturn(enabled);
        }

        public static void SetEnabled(this Firearm firearm, IEnumerable<AttachmentType> attachments)
        {
            for (int i = 0; i < firearm.Attachments.Length; i++)
            {
                if (attachments.Contains((AttachmentType)firearm.Attachments[i].Name))
                    firearm.Attachments[i].IsEnabled = true;
                else
                    firearm.Attachments[i].IsEnabled = false;
            }
        }
    }
}