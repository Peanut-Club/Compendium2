using System;

using UnityEngine;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;

using CentralAuth;

using Compendium.API.Inventory.InventoryItems;

namespace Compendium.API.Inventory
{
    public static class InventoryUtils
    {

        public static InventoryItem ToAPIItem(ItemBase item)
        {

        }

        public static void ChangeOwner(ItemBase itemBase, ReferenceHub newOwner)
        {
            if (itemBase.Owner != null)
                itemBase.OwnerInventory.ServerRemoveItem(itemBase.ItemSerial, itemBase.PickupDropModel);

            newOwner.inventory.UserInventory.Items[itemBase.ItemSerial] = itemBase;
            newOwner.inventory.ServerSendItems();

            if (itemBase is IAcquisitionConfirmationTrigger confirmationTrigger)
            {
                confirmationTrigger.ServerConfirmAcqusition();
                confirmationTrigger.AcquisitionAlreadyReceived = true;
            }
        }

        public static TPickup GetVanillaPickup<TPickup>(ItemType item, ushort serial = 0, ReferenceHub owner = null) where TPickup : ItemPickupBase
        {
            if (!InventoryItemLoader.TryGetItem<ItemBase>(item, out var itemBase))
                throw new Exception($"Failed to retrieve item '{item}'.");

            if (itemBase.PickupDropModel is null)
                throw new Exception($"Pickup model of item '{item}' is null");

            if (serial == 0)
                serial = ItemSerialGenerator.GenerateNext();

            var itemPickup = UnityEngine.Object.Instantiate(itemBase.PickupDropModel);
            var itemSync = new PickupSyncInfo(item, itemBase.Weight, serial);

            itemPickup.NetworkInfo = itemSync;

            return itemPickup as TPickup;
        }

        public static TItem GetVanillaItem<TItem>(ItemType item, ushort serial = 0, ReferenceHub owner = null) where TItem : ItemBase
        {
            if (!InventoryItemLoader.TryGetItem<TItem>(item, out var itemBase))
                throw new Exception($"Failed to retrieve item '{item}'.");

            if (serial == 0)
                serial = ItemSerialGenerator.GenerateNext();

            var itemInstance = UnityEngine.Object.Instantiate(itemBase);

            itemInstance.transform.localPosition = Vector3.zero;
            itemInstance.transform.localRotation = Quaternion.identity;

            itemInstance.Owner = owner ?? ReferenceHub.HostHub;
            itemInstance.ItemSerial = serial;

            itemInstance.OnAdded(null);

            if (itemInstance is IAcquisitionConfirmationTrigger confirmationTrigger)
            {
                confirmationTrigger.ServerConfirmAcqusition();
                confirmationTrigger.AcquisitionAlreadyReceived = true;
            }

            if (itemInstance is Firearm firearm)
            {
                if (owner.Mode != ClientInstanceMode.ReadyClient ||
                    !(AttachmentsServerHandler.PlayerPreferences.TryGetValue(owner, out var ownerAttachments)
                    && ownerAttachments.TryGetValue(firearm.ItemTypeId, out var attachmentsCode)))
                    attachmentsCode = AttachmentsUtils.GetRandomAttachmentsCode(firearm.ItemTypeId);

                firearm.ApplyAttachmentsCode(attachmentsCode, true);

                var statusFlags = FirearmStatusFlags.MagazineInserted;

                if (firearm.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                    statusFlags |= FirearmStatusFlags.FlashlightEnabled;

                firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, statusFlags, firearm.GetCurrentAttachmentsCode());
            }

            if (itemInstance.Owner != null && itemInstance.Owner.Mode is ClientInstanceMode.ReadyClient)
            {
                itemInstance.OwnerInventory.UserInventory.Items[itemInstance.ItemSerial] = itemInstance;
                itemInstance.OwnerInventory.ServerSendItems();
            }

            return itemInstance;
        }
    }
}
