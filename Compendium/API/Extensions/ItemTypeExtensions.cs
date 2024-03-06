
using Compendium.API.Enums;

namespace Compendium.API.Extensions
{
    public static class ItemTypeExtensions
    {
        public static bool IsArmor(this ItemType itemType)
            => itemType is ItemType.ArmorCombat || itemType is ItemType.ArmorHeavy || itemType is ItemType.ArmorLight;

        public static bool IsAmmo(this ItemType itemType)
            => itemType is ItemType.Ammo12gauge || itemType is ItemType.Ammo44cal || itemType is ItemType.Ammo556x45 || itemType is ItemType.Ammo762x39 || itemType is ItemType.Ammo9x19;

        public static bool IsKeycard(this ItemType itemType)
            => itemType is ItemType.KeycardJanitor || itemType is ItemType.KeycardScientist || itemType is ItemType.KeycardResearchCoordinator || itemType is ItemType.KeycardZoneManager || itemType is ItemType.KeycardGuard || itemType is ItemType.KeycardMTFPrivate || itemType is ItemType.KeycardContainmentEngineer || itemType is ItemType.KeycardMTFOperative || itemType is ItemType.KeycardMTFCaptain || itemType is ItemType.KeycardFacilityManager || itemType is ItemType.KeycardChaosInsurgency || itemType is ItemType.KeycardChaosInsurgency;

        public static bool IsWeapon(this ItemType itemType)
            => itemType is ItemType.GunA7 || itemType is ItemType.GunAK || itemType is ItemType.GunCOM15 || itemType is ItemType.GunCOM18 || itemType is ItemType.GunCom45 || itemType is ItemType.GunCrossvec || itemType is ItemType.GunE11SR || itemType is ItemType.GunFRMG0 || itemType is ItemType.GunFSP9 || itemType is ItemType.GunLogicer || itemType is ItemType.GunRevolver || itemType is ItemType.GunShotgun || itemType is ItemType.MicroHID || itemType is ItemType.ParticleDisruptor;

        public static ItemType ToItem(this AmmoType ammoType)
            => (ItemType)ammoType;

        public static ItemType ToItem(this WeaponType weapon)
            => (ItemType)weapon;

        public static ItemType ToItem(this KeycardType keycard)
            => (ItemType)keycard;

        public static ItemType ToItem(this ArmorType armorType)
            => (ItemType)armorType;

        public static KeycardType ToKeycard(this ItemType item)
            => item.IsKeycard() ? (KeycardType)item : KeycardType.None;

        public static WeaponType ToWeapon(this ItemType item)
            => item.IsWeapon() ? (WeaponType)item : WeaponType.None;

        public static AmmoType ToAmmo(this ItemType item)
            => item.IsAmmo() ? (AmmoType)item : AmmoType.None;

        public static ArmorType ToArmor(this ItemType item)
            => item.IsArmor() ? (ArmorType)item : ArmorType.None;
    }
}