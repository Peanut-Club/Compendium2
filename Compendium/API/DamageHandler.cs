using Compendium.API.Core;
using Compendium.API.Enums;
using Compendium.API.Extensions;

using PlayerStatsSystem;
using PlayerRoles.PlayableScps.Scp3114;

using Footprinting;

using UnityEngine;

using System.Collections.Generic;

namespace Compendium.API
{
    public class DamageHandler : Wrapper<StandardDamageHandler>
    {
        private static readonly Dictionary<DamageType, ItemType> DamageToItem = new Dictionary<DamageType, ItemType>()
        {
            [DamageType.Com15] = ItemType.GunCOM15,
            [DamageType.Com18] = ItemType.GunCOM18,
            [DamageType.Com45] = ItemType.GunCom45,

            [DamageType.A7] = ItemType.GunA7,
            [DamageType.Ak] = ItemType.GunAK,

            [DamageType.Fsp9] = ItemType.GunFSP9,
            [DamageType.FrMg0] = ItemType.GunFRMG0,

            [DamageType.Epsilon11] = ItemType.GunE11SR,
            [DamageType.Disruptor] = ItemType.ParticleDisruptor,
            [DamageType.Revolver] = ItemType.GunRevolver,
            [DamageType.Jailbird] = ItemType.Jailbird,
            [DamageType.Crossvec] = ItemType.GunCrossvec,
            [DamageType.Logicer] = ItemType.GunLogicer,
            [DamageType.Shotgun] = ItemType.GunShotgun
        };

        public DamageHandler(StandardDamageHandler baseValue) : base(baseValue)
        {
            Type = baseValue.GetDamageType();
            Weapon = baseValue is FirearmDamageHandler firearmDamageHandler ? firearmDamageHandler.WeaponType.ToWeapon() : WeaponType.None;
        }

        public DamageType Type { get; }
        public WeaponType Weapon { get; }

        public Player Attacker
        {
            get => Player.Get((Base as AttackerDamageHandler)?.Attacker.Hub);
            set => (Base as AttackerDamageHandler)!.Attacker = new Footprint(value.Base);
        }

        public Vector3 Velocity
        {
            get => Base.StartVelocity;
            set => Base.StartVelocity = value;
        }

        public DeathTranslation Translation
        {
            get => Base is UniversalDamageHandler universal ? DeathTranslations.TranslationsById[universal.TranslationId] : DeathTranslations.Unknown;
            set => (Base as UniversalDamageHandler)?.ApplyTranslation(value);
        }

        public HitboxType Hitbox
        {
            get => Base.Hitbox;
            set => Base.Hitbox = value;
        }

        public string CustomReason
        {
            get => Base is CustomReasonDamageHandler custom ? custom._deathReason : null;
            set => (Base as CustomReasonDamageHandler)!._deathReason = value;
        }

        public string Text
        {
            get => Base.ServerLogsText;
        }

        public float Damage
        {
            get => Base.Damage;
            set => Base.Damage = value;
        }

        public float AppliedDamage
        {
            get => Base.DealtHealthDamage;
        }

        public float AbsorbedByHumeShield
        {
            get => Base.AbsorbedHumeDamage;
        }

        public float AbsorbedByArtificial
        {
            get => Base.AbsorbedAhpDamage;
        }

        public bool IsHeadShot
        {
            get => Hitbox is HitboxType.Headshot;
        }

        public bool Apply(Player target)
            => Base.ApplyDamage(target.Base) is DamageHandlerBase.HandlerOutput.Death;

        public static DamageHandler Get(DamageType type, float damage = -1f, Player attacker = null, string reason = null, int penetration = 0, Vector3 force = default)
        {
            StandardDamageHandler handlerBase = null;

            switch (type)
            {
                case DamageType.AlphaWarhead:
                    handlerBase = new WarheadDamageHandler();
                    break;

                case DamageType.Explosion:
                    handlerBase = new ExplosionDamageHandler(new Footprint(attacker?.Base), force, damage, penetration);
                    break;

                case DamageType.Recontained:
                    handlerBase = new RecontainmentDamageHandler(new Footprint(attacker?.Base));
                    break;

                case DamageType.Scp049:
                    handlerBase = new Scp049DamageHandler(new Footprint(attacker?.Base), damage, damage == -1 ? Scp049DamageHandler.AttackType.Instakill : Scp049DamageHandler.AttackType.CardiacArrest);
                    break;

                case DamageType.Zombie:
                    handlerBase = new Scp049DamageHandler(new Footprint(attacker?.Base), damage, Scp049DamageHandler.AttackType.Scp0492);
                    break;

                case DamageType.Scp173:
                    handlerBase = new ScpDamageHandler(attacker?.Base, damage, DeathTranslations.Scp173);
                    break;

                case DamageType.Scp3114:
                    handlerBase = new ScpDamageHandler(attacker?.Base, damage, DeathTranslations.Scp3114Slap);
                    break;

                case DamageType.Scp939Lunge:
                    handlerBase = new ScpDamageHandler(attacker?.Base, damage, DeathTranslations.Scp939Lunge);
                    break;

                case DamageType.Scp939Other:
                    handlerBase = new ScpDamageHandler(attacker?.Base, damage, DeathTranslations.Scp939Other);
                    break;

                case DamageType.Scp3114Slap:
                    handlerBase = new Scp3114DamageHandler(attacker?.Base, damage, Scp3114DamageHandler.HandlerType.Slap);
                    break;

                case DamageType.Scp3114Strangulation:
                    handlerBase = new Scp3114DamageHandler(attacker?.Base, damage, Scp3114DamageHandler.HandlerType.Strangulation);
                    break;

                case DamageType.Custom:
                    handlerBase = new CustomReasonDamageHandler(reason);
                    break;

                case DamageType.Scp018:
                    {
                        var ballHandler = new Scp018DamageHandler(null, damage, false);

                        ballHandler.Attacker = new Footprint(attacker?.Base);
                        ballHandler.Damage = damage;
                        ballHandler.StartVelocity = force;

                        handlerBase = ballHandler;
                        break;
                    }

                case DamageType.Scp096:
                    {
                        var scpHandler = new Scp096DamageHandler();

                        scpHandler.Damage = damage;
                        scpHandler.Attacker = new Footprint(attacker?.Base);

                        handlerBase = scpHandler;
                        break;
                    }

                case DamageType.MicroHid:
                    {
                        var microHandler = new MicroHidDamageHandler(null, damage);

                        microHandler.Attacker = new Footprint(attacker?.Base);
                        microHandler.Damage = damage;

                        handlerBase = microHandler;
                        break;
                    }


                default:
                    {
                        if (DamageToItem.TryGetValue(type, out var firearmType))
                        {
                            var firearmHandler = new FirearmDamageHandler();

                            firearmHandler.SetWeapon(firearmType);

                            firearmHandler.Damage = damage;
                            firearmHandler.Attacker = new Footprint(attacker?.Base);

                            handlerBase = firearmHandler;
                        }
                        else
                        {
                            if (attacker != null)
                                handlerBase = new ScpDamageHandler(attacker.Base, damage, DeathTranslations.TranslationsById[(byte)type]);
                            else
                                handlerBase = new UniversalDamageHandler(damage, DeathTranslations.TranslationsById[(byte)type]);
                        }

                        break;
                    }
            }

            return new DamageHandler(handlerBase);
        }
    }
}