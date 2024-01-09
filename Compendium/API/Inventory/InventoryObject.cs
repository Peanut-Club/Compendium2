namespace Compendium.API.Inventory
{
    public class InventoryObject
    {
        public virtual ushort Serial { get; set; }
        public virtual ItemType Type { get; set; }
    }
}