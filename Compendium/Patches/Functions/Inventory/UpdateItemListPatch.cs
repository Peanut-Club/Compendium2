using InventorySystem.Items;

using System;

namespace Compendium.Patches.Functions.Inventory
{
    public static class UpdateItemListPatch
    {
        public static event Action<ItemBase> OnItemCreated;
        public static event Action<ItemBase> OnItemDestroyed;
    }
}
