using Common.Values;

using Compendium.API.Enums;

using Compendium.API.Roles.Other;
using Compendium.API.Roles.Scp096Api.Abilities;

using System.Collections.Generic;

using UnityEngine;

namespace Compendium.API.Roles.Scp096Api
{
    public class Scp096 : SubroutinedRole, IWrapper<PlayerRoles.PlayableScps.Scp096.Scp096Role>
    {
        public Scp096(PlayerRoles.PlayableScps.Scp096.Scp096Role scpRole) : base(scpRole)
        {
            Base = scpRole;

            Head = (scpRole.FpcModule.CharacterModelInstance as PlayerRoles.PlayableScps.Scp096.Scp096CharacterModel).Head;
            HeadSize = 0.12f;

            TryNotToCryAbility = GetRoutine<Scp096TryNotToCryAbility>();
            PryGateAbility = GetRoutine<Scp096PryGateAbility>();
            AttackAbility = GetRoutine<Scp096AttackAbility>();
            ChargeAbility = GetRoutine<Scp096ChargeAbility>(); 
            AudioPlayer = GetRoutine<Scp096AudioPlayer>();

            scpRole.SubroutineModule.TryGetSubroutine<PlayerRoles.PlayableScps.Scp096.Scp096RageManager>(out var scp096RageManager);

            Rage = new Scp096Rage(Player, scp096RageManager);
        }

        public new PlayerRoles.PlayableScps.Scp096.Scp096Role Base { get; }

        public Scp096TryNotToCryAbility TryNotToCryAbility { get; }
        public Scp096PryGateAbility PryGateAbility { get; }
        public Scp096AttackAbility AttackAbility { get; }
        public Scp096ChargeAbility ChargeAbility { get; }
        public Scp096AudioPlayer AudioPlayer { get; }
        public Scp096Rage Rage { get; }

        public Transform Head { get; }

        public IEnumerable<Player> Targets
        {
            get => Rage.Targets;
            set => Rage.Targets = value;
        }

        public Vector3 HeadPosition
        {
            get => Head.position;
        }

        public Quaternion HeadRotation
        {
            get => Head.rotation;
        }

        public Scp096RageType RageState
        {
            get => Rage.State;
            set => Rage.State = value;
        }

        public Scp096State State
        {
            get => (Scp096State)Base.StateController.AbilityState;
            set => Base.StateController.AbilityState = (PlayerRoles.PlayableScps.Scp096.Scp096AbilityState)value;
        }

        public PlayerRoles.PlayableScps.Scp096.Scp096HitResult SyncedAudio
        {
            get => AudioPlayer.SyncedAudio;
            set => AudioPlayer.SyncedAudio = value;
        }

        public float HeadSize { get; }

        public bool CanAttack
        {
            get => AttackAbility.CanAttack;
        }

        public bool CanCharge
        {
            get => ChargeAbility.CanCharge;
        }

        public bool CanReceiveTargets
        {
            get => Rage.CanReceiveTargets;
        }

        public bool CanAddRageDuration
        {
            get => Rage.CanAddDuration;
            set => Rage.CanAddDuration = value;
        }

        public bool IsPrying
        {
            get => State is Scp096State.PryingGate;
        }

        public bool IsAttacking
        {
            get => State is Scp096State.Attacking;
        }

        public bool IsCharging
        {
            get => State is Scp096State.Charging;
        }

        public bool IsTryingNotToCry
        {
            get => State is Scp096State.TryingNotToCry;
        }

        public bool IsDocile
        {
            get => Rage.State is Scp096RageType.Docile;
        }

        public bool IsCalming
        {
            get => Rage.State is Scp096RageType.Calming;
        }

        public bool IsDistressed
        {
            get => Rage.IsDistressed;
        }

        public bool IsCrying
        {
            get => !TryNotToCryAbility.IsActive;
            set => TryNotToCryAbility.IsActive = !value;
        }

        public bool IsLeftHandedAttack
        {
            get => AttackAbility.IsLeftHand;
            set => AttackAbility.IsLeftHand = value;
        }

        public bool IsEnraged
        {
            get => Rage.IsEnraged;
            set => Rage.IsEnraged = value;
        }

        public void Attack(bool isLeftHand = true)
            => AttackAbility.ForceAttack(isLeftHand);

        public void Attack(Player target, float damage, bool isLeftHand = true)
            => AttackAbility.ForceAttack(target, damage, isLeftHand);

        public void Charge()
            => ChargeAbility.ForceCharge();

        public void SendAudio()
            => AudioPlayer.SendAudio();

        public void SendAudio(PlayerRoles.PlayableScps.Scp096.Scp096HitResult hitResult)
            => AudioPlayer.SendAudio(hitResult);

        public void Enrage(float duration = 20f)
            => Rage.Enrage(duration);

        public void EndEnrage(bool clearTime = true)
            => Rage.EndEnrage(clearTime);

        public void AddRageDuration(float duration = 3f)
            => Rage.AddDuration(duration);
    }
}