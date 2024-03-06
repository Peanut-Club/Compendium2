using Common.Extensions;
using Common.IO.Collections;

using Compendium.API.Core;
using Compendium.API.Extensions;
using Compendium.API.Enums;
using Compendium.API.Inventory.Items.Armors;
using Compendium.API.Inventory.Items;

using InventorySystem.Items;

using System;
using System.Linq;

namespace Compendium.API.Inventory
{
    public class InventoryItem : Wrapper<ItemBase>
    {
        private static readonly LockedDictionary<ItemBase, InventoryItem> baseToItem = new LockedDictionary<ItemBase, InventoryItem>();

        internal Player owner;

        public static ushort NextSerial
        {
            get => ItemSerialGenerator.GenerateNext();
        }

        public InventoryItem(ItemBase baseValue) : base(baseValue)
        {
            if (baseValue is null)
                throw new ArgumentNullException(nameof(baseValue));

            baseToItem[baseValue] = this;
            owner = Player.Get(baseValue.Owner);
        }

        public Player Owner
        {
            get => owner;
        }

        public ItemTierFlags TierFlags
        {
            get => Base.TierFlags;
        }

        public ItemCategory Category
        {
            get => Base.Category;
        }

        public ItemType Type
        {
            get => Base.ItemTypeId;
        }

        public bool IsEquipped
        {
            get => Base.IsEquipped;
        }

        public bool IsKeycard
        {
            get => Type.IsKeycard();
        }

        public bool IsAmmo
        {
            get => Type.IsAmmo();
        }

        public bool IsArmor
        {
            get => Type.IsArmor();
        }

        public int Slot
        {
            get => owner?.Base.inventory.UserInventory.Items.FindKeyIndex(Serial) + 1 ?? -1;
            set
            {
                if (owner is null || value < 0 || value > 8 || value == Slot)
                    return;

                var atSlot = Slot;
                var atIndex = owner.Base.inventory.UserInventory.Items.ElementAtOrDefault(value);

                if (atIndex.Value != null)
                    owner.Base.inventory.UserInventory.Items.SetIndex(atSlot, atIndex.Key, atIndex.Value);

                owner.Base.inventory.UserInventory.Items.SetIndex(value, Serial, Base);
                owner.Base.inventory.ServerSendItems();
            }
        }

        public ushort Serial
        {
            get => Base.ItemSerial;
        }

        public float Weight
        {
            get => Base.Weight;
        }

        public static TItem Get<TItem>(ItemBase item) where TItem : InventoryItem
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            switch (item)
            {
                case InventorySystem.Items.Armor.BodyArmor bodyArmor:
                    {
                        switch (bodyArmor.ItemTypeId.ToArmor())
                        {
                            case ArmorType.Light:
                                return new LightBodyArmorItem(bodyArmor) as TItem;

                            case ArmorType.Heavy:
                                return new HeavyBodyArmorItem(bodyArmor) as TItem;

                            case ArmorType.Combat:
                                return new CombatBodyArmorItem(bodyArmor) as TItem;

                            default:
                                return new BodyArmorItem(bodyArmor) as TItem;
                        }
                    }

                default:
                    return new InventoryItem(item) as TItem;
            }
        }
    }
}