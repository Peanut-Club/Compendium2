using Common.Values;

using InventorySystem.Items.Pickups;

namespace Compendium.API.Inventory.WorldItems
{
    public class WorldItem : IWrapper<ItemPickupBase>
    {
        public WorldItem(ItemPickupBase pickupBase)
        {
            Base = pickupBase;
        }

        public ItemPickupBase Base { get; }
    }
}