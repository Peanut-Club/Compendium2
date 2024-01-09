namespace Compendium.API.Extensions
{
    public static class ItemTypeExtensions
    {
        public static bool IsAmmo(this ItemType itemType)
            => itemType is ItemType.Ammo12gauge || itemType is ItemType.Ammo44cal || itemType is ItemType.Ammo556x45 || itemType is ItemType.Ammo762x39 || itemType is ItemType.Ammo9x19;
    }
}