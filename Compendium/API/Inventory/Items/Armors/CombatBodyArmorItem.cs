using InventorySystem.Items.Armor;

using Compendium.API.Enums;

namespace Compendium.API.Inventory.Items.Armors
{
    public class CombatBodyArmorItem : BodyArmorItem
    {
        public CombatBodyArmorItem(BodyArmor baseValue) : base(baseValue) { }
        public CombatBodyArmorItem() : base(ArmorType.Combat) { }
    }
}