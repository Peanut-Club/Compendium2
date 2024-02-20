using Common.Extensions;
using Common.IO.Collections;

using Compendium.API.Core;

using System.Collections.Generic;
using System.Linq;

namespace Compendium.API.Inventory
{
    public class InventoryManager : PlayerWrapper<InventorySystem.Inventory>
    {
        private readonly LockedDictionary<ushort, InventoryItem> items;

        public InventoryManager(Player player, InventorySystem.Inventory baseValue) : base(player, baseValue)
        {
            items = new LockedDictionary<ushort, InventoryItem>(8);
            RefreshItems();
        }

        public IEnumerable<InventoryItem> Items
        {
            get => items.Values;
        }

        public TItem[] Get<TItem>() where TItem : InventoryItem
            => items.Values.Where(item => item is TItem).CastArray<TItem>();

        public TItem Get<TItem>(ushort id) where TItem : InventoryItem
            => items.TryGetValue(id, out var item) ? (TItem)item : null;

        internal void RefreshItems()
        {
            items.Clear();

            foreach (var item in Base.UserInventory.Items)
            {
                var invItem = InventoryItem.Get<InventoryItem>(item.Value);

                if (invItem is null)
                    continue;

                items[item.Key] = invItem;
            }

            Base.UserInventory.Items.Clear();

            foreach (var item in items)
                Base.UserInventory.Items[item.Key] = item.Value.Base;

            Base.ServerSendItems();
        }
    }
}