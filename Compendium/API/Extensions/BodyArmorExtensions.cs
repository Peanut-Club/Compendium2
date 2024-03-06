using Compendium.API.Enums;
using InventorySystem.Items.Armor;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Extensions
{
    public static class BodyArmorExtensions
    {
        public static BodyArmor.ArmorCategoryLimitModifier[] ToBodyArmorCategoryLimits(this IDictionary<ItemCategory, byte> limits)
        {
            var array = new BodyArmor.ArmorCategoryLimitModifier[limits.Count];

            for (int i = 0; i < limits.Count; i++)
            {
                var pair = limits.ElementAt(i);

                array[i] = new BodyArmor.ArmorCategoryLimitModifier
                {
                    Category = pair.Key,
                    Limit = pair.Value
                };
            }

            return array;
        }

        public static BodyArmor.ArmorAmmoLimit[] ToBodyArmorLimits(this IDictionary<AmmoType, ushort> limits)
        {
            var array = new BodyArmor.ArmorAmmoLimit[limits.Count];

            for (int i = 0; i < limits.Count; i++)
            {
                var pair = limits.ElementAt(i);

                array[i] = new BodyArmor.ArmorAmmoLimit
                {
                    AmmoType = pair.Key.ToItem(),
                    Limit = pair.Value
                };
            }

            return array;
        }

        public static Dictionary<ItemCategory, byte> ToBodyArmorCategoryLimitsDict(this IEnumerable<BodyArmor.ArmorCategoryLimitModifier> armorCategoryLimits)
        {
            var dict = new Dictionary<ItemCategory, byte>(armorCategoryLimits.Count());

            foreach (var limit in armorCategoryLimits)
                dict[limit.Category] = limit.Limit;

            return dict;
        }

        public static Dictionary<AmmoType, ushort> ToBodyArmorLimitsDict(this IEnumerable<BodyArmor.ArmorAmmoLimit> armorAmmoLimits)
        {
            var dict = new Dictionary<AmmoType, ushort>(armorAmmoLimits.Count());

            foreach (var limit in armorAmmoLimits)
                dict[limit.AmmoType.ToAmmo()] = limit.Limit;

            return dict;
        }
    }
}