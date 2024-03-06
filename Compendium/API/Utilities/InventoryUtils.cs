using Compendium.API.Inventory;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;

using UnityEngine;

namespace Compendium.API.Utilities
{
    public static class InventoryUtils
    {
        public static T GetInstance<T>(this ItemType item, ushort serial = 0, ReferenceHub owner = null, bool insertMagazineIfFirearm = false) where T : ItemBase
        {
            if (item is ItemType.None || !InventoryItemLoader.TryGetItem<T>(item, out var result))
                return null;

            if (serial is 0)
                serial = InventoryItem.NextSerial;

            var instance = Object.Instantiate(result);

            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;

            instance.Owner = owner;
            instance.ItemSerial = serial;

            if (instance is Firearm firearm)
                firearm.SetupFirearm(insertMagazineIfFirearm);

            return instance;
        }

        public static void SetupFirearm(this Firearm firearm, bool insertMagazine = false)
        {
            if (firearm.Owner != null
                && AttachmentsServerHandler.PlayerPreferences.TryGetValue(firearm.Owner, out var preferences)
                && preferences.TryGetValue(firearm.ItemTypeId, out var preferencesCode))
                firearm.ApplyAttachmentsCode(preferencesCode, true);
            else
                firearm.ApplyAttachmentsCode(AttachmentsUtils.GetRandomAttachmentsCode(firearm.ItemTypeId), true);

            var flags = FirearmStatusFlags.MagazineInserted;

            if (firearm.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                flags |= FirearmStatusFlags.FlashlightEnabled;

            if (insertMagazine)
                flags |= FirearmStatusFlags.MagazineInserted;

            firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, flags, firearm.GetCurrentAttachmentsCode());
        }
    }
}