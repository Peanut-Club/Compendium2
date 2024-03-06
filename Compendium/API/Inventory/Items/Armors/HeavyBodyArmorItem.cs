using InventorySystem.Items.Armor;

using Compendium.API.Enums;

namespace Compendium.API.Inventory.Items.Armors
{
    public class HeavyBodyArmorItem : BodyArmorItem
    {
        public HeavyBodyArmorItem(BodyArmor baseValue) : base(baseValue) { }
        public HeavyBodyArmorItem() : base(ArmorType.Heavy) { }
    }
}