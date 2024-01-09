using Common.Extensions;

using Compendium.API.Extensions;
using Compendium.API.Inventory.InventoryItems;

using Compendium.Patches.Functions.Inventory;

using InventorySystem.Items;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Pickups;

using Mirror;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Compendium.API.Inventory
{
    public class InventoryManager
    {
        private static readonly Dictionary<ItemBase, InventoryItem> vannilaToApi;

        static InventoryManager()
        {
            vannilaToApi = new Dictionary<ItemBase, InventoryItem>();

            UpdateItemListPatch.OnItemCreated += OnItemCreated;
            UpdateItemListPatch.OnItemDestroyed += OnItemDestroyed;
            UpdateItemOwnerPatch.OnItemOwnerChanged += OnItemOwnerChanged;
        }

        private List<InventoryItem> items;

        public InventoryManager(Player owner)
        {
            Owner = owner;
            AmmoValidation = true;

            items = new List<InventoryItem>(8);
        }

        public Player Owner { get; }

        public InventoryItem Current
        {
            get => Get<InventoryItem>(Owner.Hub.inventory.CurInstance);
            set => Owner.Hub.inventory.CurInstance = value.Base;
        }

        public ushort CurrentSerial
        {
            get => Owner.Hub.inventory.NetworkCurItem.SerialNumber;
            set => Owner.Hub.inventory.ServerSelectItem(value);
        }

        public ItemType CurrentType
        {
            get => Owner.Hub.inventory.NetworkCurItem.TypeId;
            set => Owner.Hub.inventory.ServerSelectItem(Owner.Hub.inventory.UserInventory.Items.First(p => p.Value.ItemTypeId == value).Key);
        }

        public IReadOnlyList<InventoryItem> Items => items;

        public Dictionary<ItemType, ushort> Ammo => Owner.Hub.inventory.UserInventory.ReserveAmmo;

        public bool AmmoValidation { get; set; }

        public InventoryObject AddOrDropItem(ItemType item)
        {
            if (Items.Count >= 8)
            {
                var pickup = InventoryUtils.GetVanillaPickup<ItemPickupBase>(item, 0, Owner.Hub);

                NetworkServer.Spawn(pickup.gameObject);

                return true;
            }
            else
            {
                var itemBase = InventoryUtils.GetVanillaItem<ItemBase>(item, 0, Owner.Hub);
                var invItem = Get<InventoryItem>(itemBase);

                if (!items.Contains(invItem))
                    items.Add(invItem);

                return invItem;
            }
        }

        public InventoryItem AddItem(ItemType item)
        {
            if (Items.Count >= 8)
                return null;

            var itemBase = InventoryUtils.GetVanillaItem<ItemBase>(item, 0, Owner.Hub);
            var invItem = Get<InventoryItem>(itemBase);

            if (!items.Contains(invItem))
                items.Add(invItem);

            return invItem;
        }

        public ushort GetAmmo(ItemType type)
            => Ammo.TryGetValue(type, out var amount) ? amount : (ushort)0;

        public int GetAllAmmo()
            => Ammo.Sum(pair => pair.Value);

        public void SetAmmo(ItemType ammoType, ushort amount)
        {
            Ammo[ammoType] = amount;
            ValidateAmmo();
        }

        public void SetAllAmmo(ushort amount)
        {
            foreach (var key in Ammo.Keys)
                Ammo[key] = amount;

            ValidateAmmo();
        }

        public void AddAmmo(ItemType ammoType, ushort amount)
        {
            if (!Ammo.ContainsKey(ammoType))
                Ammo[ammoType] = amount;
            else
            {
                FixAmount(Ammo[ammoType], ref amount);
                Ammo[ammoType] += amount;
            }

            ValidateAmmo();
        }

        public void AddAllAmmo(ushort amount)
        {
            foreach (var key in Ammo.Keys)
            {
                FixAmount(Ammo[key], ref amount);
                Ammo[key] += amount;
            }

            ValidateAmmo();
        }

        public void SubstractAmmo(ItemType ammoType, ushort amount)
        {
            if (!Ammo.ContainsKey(ammoType))
                return;

            if (Ammo[ammoType] <= amount)
                Ammo[ammoType] = 0;
            else
            {
                FixAmount(Ammo[ammoType], ref amount, true);
                Ammo[ammoType] -= amount;
            }

            ValidateAmmo();
        }

        public void SubstractAllAmmo(ushort amount)
        {
            foreach (var key in Ammo.Keys)
            {
                FixAmount(Ammo[key], ref amount, true);
                Ammo[key] -= amount;
            }

            ValidateAmmo();
        }

        public void RemoveAmmo(ItemType ammoType)
        {
            if (Ammo.Remove(ammoType))
                ValidateAmmo();
        }

        public void RemoveAllAmmo()
        {
            if (Ammo.Count == 0)
                return;

            Ammo.Clear();
            ValidateAmmo();
        }

        public void DropAmmo(ItemType ammoType, ushort amount)
        {
            if (!Ammo.ContainsKey(ammoType) || Ammo[ammoType] == 0 || !(AmmoValidation && !ammoType.IsAmmo()))
                return;

            if (amount > Ammo[ammoType])
                amount = Ammo[ammoType];

            var ammoPickup = InventoryUtils.GetVanillaPickup<ItemPickupBase>(ammoType, 0, Owner.Hub);

            if (ammoPickup is null)
                return;

            if (ammoPickup is AmmoPickup ammo)
                ammo.NetworkSavedAmmo = amount;

            ammoPickup.Position = Owner.Position;
            ammoPickup.Rotation = Owner.Rotation;

            NetworkServer.Spawn(ammoPickup.gameObject);

            Ammo[ammoType] -= amount;

            ValidateAmmo();
        }

        public void DropAllAmmo(ItemType ammoType)
        {
            if (!Ammo.TryGetValue(ammoType, out var amount))
                return;

            DropAmmo(ammoType, amount);
        }

        public void DropAllAmmo()
        {
            foreach (var key in Ammo.Keys)
                DropAmmo(key, Ammo[key]);
        }

        public void ValidateAmmo()
        {
            if (AmmoValidation)
                Ammo.RemoveKeys(ammoType => !ammoType.IsAmmo());

            Owner.Hub.inventory.ServerSendAmmo();
        }

        public static TItem Get<TItem>(ItemBase item) where TItem : InventoryItem
        {
            if (item is null)
                return default;

            if (vannilaToApi.TryGetValue(item, out var invItem))
            {
                if (invItem is not TItem tInvItem)
                    throw new Exception($"Failed to cast item '{invItem.GetType().FullName}' to '{typeof(TItem).FullName}'");

                return tInvItem;
            }

            return (TItem)(vannilaToApi[item] = InventoryUtils.ToAPIItem(item));
        }

        private static void FixAmount(ushort curAmount, ref ushort amount, bool substract = false)
        {
            if ((curAmount + amount) > ushort.MaxValue)
                amount = (ushort)(ushort.MaxValue - curAmount);
            else if (substract)
                while ((curAmount - amount) < 0)
                    amount--;
        }

        private static void OnItemDestroyed(ItemBase item)
        {
            if (vannilaToApi.TryGetValue(item, out var invItem)
                && invItem.Inventory != null)
                invItem.Inventory.items.Remove(invItem);

            vannilaToApi.Remove(item);
        }

        private static void OnItemCreated(ItemBase item)
        {
            var invItem = vannilaToApi[item] = InventoryUtils.ToAPIItem(item);

            if (invItem.Inventory != null && !invItem.Inventory.items.Contains(invItem))
                invItem.Inventory.items.Add(invItem);
        }

        private static void OnItemOwnerChanged(ItemBase item, ReferenceHub oldOwner, ReferenceHub newOwner)
        {
            if (vannilaToApi.TryGetValue(item, out var invItem))
            {
                var oldPlayer = Player.Get(oldOwner);
                var newPlayer = Player.Get(newOwner);

                oldPlayer?.Inventory.items.Remove(invItem);
                newPlayer?.Inventory.items.Add(invItem);
            }
            else
            {
                invItem = vannilaToApi[item] = InventoryUtils.ToAPIItem(item);
                invItem.Inventory?.items.Add(invItem);
            }
        }
    }
}