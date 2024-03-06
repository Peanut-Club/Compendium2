using Common.Values;
using Common.Extensions;
using Common.Logging;

using Compendium.API.Enums;
using Compendium.API.Utilities;
using Compendium.API.Extensions;
using Compendium.API.Inventory.Items.Firearms.Modules;

using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Attachments;

using PlayerRoles;

using System.Linq;

namespace Compendium.API.Inventory.Items
{
    public class FirearmItem : InventoryItem, IWrapper<Firearm>
    {
        public static LogOutput Log { get; set; } = new LogOutput("Firearm API").Setup();

        public FirearmItem(Firearm baseValue) : base(baseValue)
        {
            Base = baseValue;

            Penetration = baseValue.ArmorPenetration;

            AdsInnacuracy = baseValue.BaseStats.AdsInaccuracy;
            HipInnacuracy = baseValue.BaseStats.HipInaccuracy;

            BaseDamage = baseValue.BaseStats.BaseDamage;

            ShouldSubstractAmmo = true;

            AmmoManager = new WrappedAmmoManager(AmmoManager);
        }

        public new Firearm Base { get; }

        public IAmmoManagerModule AmmoManager
        {
            get => Base.AmmoManagerModule;
            set => Base.AmmoManagerModule = value;
        }

        public AmmoType AmmoType
        {
            get => Base.AmmoType.ToAmmo();
        }

        public Faction Affiliation
        {
            get => Base.FirearmAffiliation;
        }

        public AttachmentType[] Attachments
        {
            get => Base.GetEnabled();
            set => Base.SetEnabled(value);
        }

        public AttachmentType Magazine
        {
            get => GetAttachment(AttachmentSlot.Ammunition);
            set => SetAttachment(AttachmentSlot.Ammunition, value);
        }

        public AttachmentType Barell
        {
            get => GetAttachment(AttachmentSlot.Barrel);
            set => SetAttachment(AttachmentSlot.Barrel, value);
        }

        public AttachmentType Body
        {
            get => GetAttachment(AttachmentSlot.Body);
            set => SetAttachment(AttachmentSlot.Body, value);
        }

        public AttachmentType BottomRail
        {
            get => GetAttachment(AttachmentSlot.BottomRail);
            set => SetAttachment(AttachmentSlot.BottomRail, value);
        }

        public AttachmentType SideRail
        {
            get => GetAttachment(AttachmentSlot.SideRail);
            set => SetAttachment(AttachmentSlot.SideRail, value);
        }

        public AttachmentType Sight
        {
            get => GetAttachment(AttachmentSlot.Sight);
            set => SetAttachment(AttachmentSlot.Sight, value);
        }

        public AttachmentType Stability
        {
            get => GetAttachment(AttachmentSlot.Stability);
            set => SetAttachment(AttachmentSlot.Stability, value);
        }

        public AttachmentType Stock
        {
            get => GetAttachment(AttachmentSlot.Stock);
            set => SetAttachment(AttachmentSlot.Stock, value);
        }

        public FirearmStatusFlags StatusFlags
        {
            get => Base.Status.Flags;
            set => Base.Status = new FirearmStatus(Ammo, value, Base.Status.Attachments);
        }

        public FirearmStatus Status
        {
            get => Base.Status;
            set => Base.Status = value;
        }

        public float Penetration { get; set; }

        public float AdsInnacuracy { get; set; }
        public float HipInnacuracy { get; set; }

        public float BaseDamage { get; set; }

        public float Length
        {
            get => Base.Length;
        }

        public float CurrentInnacuracy
        {
            get => Base.BaseStats.GetInaccuracy(Base, IsAds, Owner.MovementSpeed, Owner.FirstPerson?.IsGrounded ?? false);
        }

        public uint AttachmentsCode
        {
            get => Base.GetCurrentAttachmentsCode();
            set
            {
                Base.ApplyAttachmentsCode(value, true);
                RefreshStatus();
            }
        }

        public byte Ammo
        {
            get => Status.Ammo;
            set => Status = new FirearmStatus(value, StatusFlags, AttachmentsCode);
        }

        public byte MaxAmmo
        {
            get => AmmoManager.MaxAmmo;
            set => ((WrappedAmmoManager)AmmoManager).MaxAmmo = value;
        }

        public bool ShouldSubstractAmmo { get; set; }

        public bool HasFlashlight
        {
            get => HasAttachment(AttachmentType.Flashlight);
            set => SetAttachment(AttachmentType.Flashlight, value);
        }

        public bool IsMagazineInserted
        {
            get => StatusFlags.HasFlagFast(FirearmStatusFlags.MagazineInserted);
            set => StatusFlags = value ? StatusFlags | FirearmStatusFlags.MagazineInserted : StatusFlags & ~FirearmStatusFlags.MagazineInserted;
        }

        public bool IsChambered
        {
            get => StatusFlags.HasFlagFast(FirearmStatusFlags.Chambered);
            set => StatusFlags = value ? StatusFlags | FirearmStatusFlags.Chambered : StatusFlags & ~FirearmStatusFlags.Chambered;
        }

        public bool IsCocked
        {
            get => StatusFlags.HasFlagFast(FirearmStatusFlags.Cocked);
            set => StatusFlags = value ? StatusFlags | FirearmStatusFlags.Cocked : StatusFlags & ~FirearmStatusFlags.Cocked;
        }

        public bool IsFlashlightEnabled
        {
            get => StatusFlags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);
            set => StatusFlags = value ? StatusFlags | FirearmStatusFlags.FlashlightEnabled : StatusFlags & ~FirearmStatusFlags.FlashlightEnabled;
        }

        public bool IsReloading
        {
            get => !Base.AmmoManagerModule.Standby;
        }

        public bool IsAds
        {
            get => Base.AdsModule.ServerAds;
            set => Base.AdsModule.ServerAds = value;
        }

        public bool HasAttachment(AttachmentType type)
            => Attachments.Contains(type);

        public bool AddAttachment(AttachmentType type)
        {
            var attachments = Attachments;

            if (attachments.Contains(type))
            {
                Log.Warn($"Failed to add attachment '{type}' - this item already has it enabled.");
                return false;
            }

            Attachments = attachments.Concat([type]).ToArray();
            return true;
        }

        public bool RemoveAttachment(AttachmentType type)
        {
            var attachments = Attachments;

            if (!attachments.Contains(type))
            {
                Log.Warn($"Failed to remove attachment '{type}' - this item does not have it enabled.");
                return false;
            }

            Attachments = attachments.WhereArray(a => a != type);
            return true;
        }

        public AttachmentType GetAttachment(AttachmentSlot slot)
        {
            if (Base.Attachments.TryGetFirst(a => a.IsEnabled && a.Slot == slot, out var enabled))
                return (AttachmentType)enabled.Name;

            return AttachmentType.None;
        }

        public void SetAttachment(AttachmentSlot slot, AttachmentType type)
        {
            var current = GetAttachment(slot);
            var attachments = Attachments.ToList();

            if (current != AttachmentType.None)
                attachments.Remove(current);

            attachments.Add(type);
            Attachments = attachments.ToArray();
        }

        public void SetAttachment(AttachmentType attachmentType, bool enabled)
        {
            var isEnabled = HasAttachment(attachmentType);

            if (isEnabled == enabled)
                return;

            if (enabled)
                AddAttachment(attachmentType);
            else
                RemoveAttachment(attachmentType);
        }

        public void RefreshStatus()
            => Status = new FirearmStatus(Ammo, StatusFlags, AttachmentsCode);

        public void RefreshAttachments()
            => AttachmentsCode = AttachmentsCode;
    }
}