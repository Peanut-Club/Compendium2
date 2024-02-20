using Compendium.API.Core;

using InventorySystem.Items;

namespace Compendium.API.Inventory
{
    public class InventoryItem : Wrapper<ItemBase>
    {
        public InventoryItem(ItemBase baseValue) : base(baseValue)
        {
        }

        public static TItem Get<TItem>(ItemBase item)
        {

        }
    }
}