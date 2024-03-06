using Common.Values;

using Compendium.API.Enums;
using Compendium.API.Extensions;
using Compendium.API.Utilities;

using System.Collections.Generic;

using InventorySystem.Items.Armor;

namespace Compendium.API.Inventory.Items
{
    public class BodyArmorItem : InventoryItem, IWrapper<BodyArmor>
    {
        public BodyArmorItem(BodyArmor baseValue) : base(baseValue)
        {
            Base = baseValue;
        }

        public BodyArmorItem(ArmorType armorType) 
            : this(armorType.ToItem().GetInstance<BodyArmor>())
        { }

        public new BodyArmor Base { get; }

        public Dictionary<AmmoType, ushort> AmmoLimits
        {
            get => Base.AmmoLimits.ToBodyArmorLimitsDict();
            set => Base.AmmoLimits = value.ToBodyArmorLimits();
        }

        public Dictionary<ItemCategory, byte> ItemLimits
        {
            get => Base.CategoryLimits.ToBodyArmorCategoryLimitsDict();
            set => Base.CategoryLimits = value.ToBodyArmorCategoryLimits();
        }

        public bool ShouldDropExcessive
        {
            get => !Base.DontRemoveExcessOnDrop;
            set => Base.DontRemoveExcessOnDrop = !value;
        }

        public bool IsHeavy
        {
            get => Type is ItemType.ArmorHeavy;
        }

        public bool IsLight
        {
            get => Type is ItemType.ArmorLight;
        }

        public bool IsCombat
        {
            get => Type is ItemType.ArmorCombat;
        }

        public int HelmetEfficacy
        {
            get => Base.HelmetEfficacy;
            set => Base.HelmetEfficacy = value;
        }

        public int VestEfficacy
        {
            get => Base.VestEfficacy;
            set => Base.VestEfficacy = value;
        }

        public float CivilianDownsidesMultiplier
        {
            get => Base.CivilianClassDownsidesMultiplier;
            set => Base.CivilianClassDownsidesMultiplier = value;
        }

        public float DamageAt(float damage, int penetration)
            => InventorySystem.Items.Armor.BodyArmorUtils.ProcessDamage(VestEfficacy, damage, penetration);
    }
}