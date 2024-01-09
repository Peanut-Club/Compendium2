using Common.Values;

using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.ToggleableLights;
using InventorySystem.Items.Usables;

using UnityEngine;

namespace Compendium.API.Inventory.InventoryItems
{
    public class InventoryItem : InventoryObject, IWrapper<ItemBase>
    {
        private ItemBase itemBase;

        private StatusValue<bool> disarmingOverride;
        private StatusValue<bool> holsterOverride;
        private StatusValue<bool> equipOverride;

        public InventoryItem(ItemBase item)
        {
            itemBase  = item;

            Weight = item.Weight;
            Serial = item.ItemSerial;

            disarmingOverride = new StatusValue<bool>(() => Base is IDisarmingItem disarmingItem && disarmingItem.AllowDisarming);
            holsterOverride = new StatusValue<bool>(() => Base is IEquipDequipModifier equipDequipModifier && equipDequipModifier.AllowHolster);
            equipOverride = new StatusValue<bool>(() => Base is IEquipDequipModifier equipDequipModifier && equipDequipModifier.AllowEquip);

            Scale = Vector3.one;
        }

        public ItemBase Base => itemBase;

        public Player Owner { get; internal set; }
        public InventoryManager Inventory => Owner?.Inventory;

        public override ItemType Type
        {
            get => Base.ItemTypeId;
            set
            {
                if (Base.ItemTypeId == value)
                    return;

                itemBase = InventoryUtils.GetVanillaItem<ItemBase>(value, Serial, Base.Owner);

                Owner.Hub.inventory.UserInventory.Items[Serial] = itemBase;
                Owner.Hub.inventory.SendItemsNextFrame = true;

                Weight = itemBase.Weight;
            }
        }


        public ItemCategory Category => Base.Category;
        public ItemTierFlags Tier => Base.TierFlags;

        public Vector3 TorqueA
        {
            get => Base.ThrowSettings.RandomTorqueA;
            set => Base.ThrowSettings.RandomTorqueA = value;
        }

        public Vector3 TorqueB
        {
            get => Base.ThrowSettings.RandomTorqueB;
            set => Base.ThrowSettings.RandomTorqueB = value;
        }

        public Vector3 Velocity
        {
            get => Vector3.Lerp(TorqueA, TorqueB, Random.value);
        }

        public bool IsEquipped
        {
            get => Base.IsEquipped;
        }

        public bool IsWorn
        {
            get => Base is IWearableItem wearableItem && wearableItem.IsWorn;
        }

        public bool IsDisarmingAllowed
        {
            get => disarmingOverride.Value;
            set => disarmingOverride.Value = value;
        }

        public bool IsHolsterAllowed
        {
            get => holsterOverride.Value;
            set => holsterOverride.Value = value;
        }

        public bool IsEquipAllowed
        {
            get => equipOverride.Value;
            set => equipOverride.Value = value;
        }

        public bool IsEmittingLight
        {
            get => Base is ILightEmittingItem lightItem && lightItem.IsEmittingLight;
            set
            {
                if (IsEmittingLight == value)
                    return;

                if (Base is ToggleableLightItemBase lightItem)
                {
                    lightItem.NextAllowedTime = Time.timeSinceLevelLoad + 0.6f;
                    lightItem.IsEmittingLight = value;
                }
                else if (Base is Firearm firearm)
                {
                    if (value && !firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled))
                        firearm.Status = new FirearmStatus(firearm.Status.Ammo, firearm.Status.Flags | FirearmStatusFlags.FlashlightEnabled, firearm.Status.Attachments);
                    else if (!value && firearm.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled))
                        firearm.Status = new FirearmStatus(firearm.Status.Ammo, firearm.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, firearm.Status.Attachments);
                }
            }
        }

        public bool IsAmmo
        {
            get => Base is AmmoItem;
        }

        public bool IsArmor
        {
            get => Base is BodyArmor;
        }

        public bool IsKeycard
        {
            get => Base is KeycardItem;
        }

        public bool IsConsumable
        {
            get => Base is Consumable;
        }

        public bool IsThrowable
        {
            get => Base is ThrowableItem;
        }

        public bool IsUsable
        {
            get => Base is UsableItem;
        }

        public bool IsWeapon
        {
            get => Base is Firearm;
        }

        public override ushort Serial
        {
            get => Base.ItemSerial;
            set => Base.ItemSerial = value;
        }

        public float Weight { get; set; }

        public Vector3 Scale { get; set; }

        internal void RefreshProperties(ItemBase item)
        {
            itemBase = item;

            Weight = item.Weight;

            Scale = Vector3.one;

            disarmingOverride.Reset();
            equipOverride.Reset();
            holsterOverride.Reset();
        }
    }
}