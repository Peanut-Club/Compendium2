using InventorySystem.Items.Armor;

using Compendium.API.Enums;

namespace Compendium.API.Inventory.Items.Armors
{
    public class LightBodyArmorItem : BodyArmorItem
    {
        public LightBodyArmorItem(BodyArmor baseValue) : base(baseValue) { }
        public LightBodyArmorItem() : base(ArmorType.Light) { }
    }
}