using InventorySystem.Items;

using System;

namespace Compendium.Patches.Functions.Inventory
{
    public static class UpdateItemOwnerPatch
    {
        public static event Action<ItemBase, ReferenceHub, ReferenceHub> OnItemOwnerChanged;
    }
}