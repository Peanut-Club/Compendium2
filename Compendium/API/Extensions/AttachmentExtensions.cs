using Compendium.API.Enums;

using InventorySystem.Items.Firearms;
using InventorySystem;

using System.Collections.Generic;
using System.Linq;

using InventorySystem.Items.Firearms.Attachments;
using System;
using Common.Pooling.Pools;

namespace Compendium.API.Extensions
{
    public static class AttachmentExtensions
    {
        public static bool IsSight(this AttachmentType attachmentType)
            => attachmentType is AttachmentType.NightVisionSight || attachmentType is AttachmentType.AmmoSight || attachmentType is AttachmentType.DotSight || attachmentType is AttachmentType.HoloSight || attachmentType is AttachmentType.IronSights || attachmentType is AttachmentType.NightVisionSight || attachmentType is AttachmentType.ScopeSight;

        public static bool IsMagazine(this AttachmentType attachmentType)
            => attachmentType is AttachmentType.CylinderMag4 || attachmentType is AttachmentType.CylinderMag6 || attachmentType is AttachmentType.CylinderMag8 || attachmentType is AttachmentType.DrumMagAP || attachmentType is AttachmentType.DrumMagFMJ || attachmentType is AttachmentType.DrumMagJHP || attachmentType is AttachmentType.ExtendedMagAP || attachmentType is AttachmentType.ExtendedMagFMJ || attachmentType is AttachmentType.ExtendedMagJHP || attachmentType is AttachmentType.LowcapMagAP || attachmentType is AttachmentType.LowcapMagFMJ || attachmentType is AttachmentType.LowcapMagJHP || attachmentType is AttachmentType.StandardMagAP || attachmentType is AttachmentType.StandardMagFMJ || attachmentType is AttachmentType.StandardMagJHP;

        public static bool IsStock(this AttachmentType attachmentType)
            => attachmentType is AttachmentType.ExtendedStock || attachmentType is AttachmentType.HeavyStock || attachmentType is AttachmentType.LightweightStock || attachmentType is AttachmentType.NoRifleStock || attachmentType is AttachmentType.RecoilReducingStock || attachmentType is AttachmentType.RetractedStock || attachmentType is AttachmentType.StandardStock;

        public static bool IsBarrel(this AttachmentType attachmentType)
            => attachmentType is AttachmentType.ExtendedBarrel || attachmentType is AttachmentType.ShortBarrel || attachmentType is AttachmentType.ShotgunExtendedBarrel || attachmentType is AttachmentType.StandardBarrel;

        public static uint ToCode(this IEnumerable<AttachmentType> attachments, WeaponType firearm)
        {
            if (firearm is WeaponType.None)
                return 0;

            var item = firearm.ToItem();

            if (!InventoryItemLoader.TryGetItem<Firearm>(item, out var firearmBase))
                return 0;

            var curCode = firearmBase.GetCurrentAttachmentsCode();

            for (int i = 0; i < firearmBase.Attachments.Length; i++)
            {
                if (attachments.Contains((AttachmentType)firearmBase.Attachments[i].Name))
                    firearmBase.Attachments[i].IsEnabled = true;
                else
                    firearmBase.Attachments[i].IsEnabled = false;
            }

            var code = firearmBase.GetCurrentAttachmentsCode();

            firearmBase.ApplyAttachmentsCode(curCode, false);

            return code;
        }

        public static AttachmentType[] FromCode(this uint attachmentsCode, WeaponType weaponType)
        {
            if (weaponType is WeaponType.None || !InventoryItemLoader.TryGetItem<Firearm>(weaponType.ToItem(), out var firearm))
                return Array.Empty<AttachmentType>();

            var code = firearm.GetCurrentAttachmentsCode();
            var attachments = ListPool<AttachmentType>.Shared.Rent();

            firearm.ApplyAttachmentsCode(attachmentsCode, true);

            for (int i = 0; i < firearm.Attachments.Length; i++)
            {
                if (firearm.Attachments[i].IsEnabled)
                    attachments.Add((AttachmentType)firearm.Attachments[i].Name);
            }

            firearm.ApplyAttachmentsCode(code, false);

            return ListPool<AttachmentType>.Shared.ToArrayReturn(attachments);
        }

        public static AttachmentSlot GetSlot(this AttachmentType type)
        {
            if (type.IsSight())
                return AttachmentSlot.Sight;

            if (type.IsMagazine())
                return AttachmentSlot.Ammunition;

            if (type.IsStock())
                return AttachmentSlot.Stock;

            if (type.IsBarrel() || type is AttachmentType.MuzzleBrake || type is AttachmentType.MuzzleBooster)
                return AttachmentSlot.Barrel;

            return AttachmentSlot.Unassigned;
        }
    }
}
