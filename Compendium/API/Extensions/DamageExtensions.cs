using PlayerRoles.PlayableScps.Scp3114;
using PlayerStatsSystem;

using Compendium.API.Enums;

namespace Compendium.API.Extensions
{
    public static class DamageExtensions
    {
        public static DamageType GetDamageType(this DamageHandlerBase damage)
        {
            switch (damage)
            {
                case WarheadDamageHandler:
                    return DamageType.AlphaWarhead;

                case RecontainmentDamageHandler:
                    return DamageType.Recontained;

                case MicroHidDamageHandler:
                    return DamageType.MicroHid;

                case JailbirdDamageHandler:
                    return DamageType.Jailbird;

                case DisruptorDamageHandler:
                    return DamageType.Disruptor;

                case CustomReasonDamageHandler:
                    return DamageType.Custom;

                case ExplosionDamageHandler:
                    return DamageType.Explosion;

                case Scp018DamageHandler:
                    return DamageType.Scp018;

                case Scp096DamageHandler:
                    return DamageType.Scp096;

                case Scp049DamageHandler scp049DamageHandler:
                    return scp049DamageHandler.DamageSubType switch
                    {
                        Scp049DamageHandler.AttackType.CardiacArrest => DamageType.CardiacArrest,
                        Scp049DamageHandler.AttackType.Scp0492 => DamageType.Zombie,

                        _ => DamageType.Scp049
                    };

                case Scp3114DamageHandler scp3114DamageHandler:
                    return scp3114DamageHandler.Subtype switch
                    {
                        Scp3114DamageHandler.HandlerType.Slap => DamageType.Scp3114Slap,
                        Scp3114DamageHandler.HandlerType.Strangulation => DamageType.Scp3114Strangulation,

                        _ => DamageType.Scp3114,
                    };

                case FirearmDamageHandler firearmDamageHandler:
                    return firearmDamageHandler.WeaponType switch
                    {
                        ItemType.GunCOM15 => DamageType.Com15,
                        ItemType.GunCOM18 => DamageType.Com18,
                        ItemType.GunA7 => DamageType.A7,
                        ItemType.GunAK => DamageType.Ak,
                        ItemType.GunFSP9 => DamageType.Fsp9,
                        ItemType.GunFRMG0 => DamageType.FrMg0,
                        ItemType.GunE11SR => DamageType.Epsilon11,
                        ItemType.ParticleDisruptor => DamageType.Disruptor,
                        ItemType.GunRevolver => DamageType.Revolver,
                        ItemType.Jailbird => DamageType.Jailbird,
                        ItemType.GunCrossvec => DamageType.Crossvec,
                        ItemType.MicroHID => DamageType.MicroHid,
                        ItemType.GunLogicer => DamageType.Logicer,
                        ItemType.GunShotgun => DamageType.Shotgun,

                        _ => DamageType.Firearm
                    };

                case UniversalDamageHandler universalDamageHandler:
                    return (DamageType)universalDamageHandler.TranslationId;

                default:
                    return DamageType.Unknown;
            }
        }
    }
}
