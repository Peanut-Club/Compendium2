using InventorySystem.Items.Firearms.Modules;

using System;

namespace Compendium.API.Inventory.Items.Firearms.Modules
{
    public class CustomAmmoManager : IAmmoManagerModule
    {
        private Func<bool> tryReloadDelegate;
        private Func<bool> tryUnloadDelegate;
        private Func<bool> tryStopReloadDelegate;

        public CustomAmmoManager(FirearmItem firearm, byte maxAmmo, Func<bool> tryReload = null, Func<bool> tryStopReload = null, Func<bool> tryUnload = null)
        {
            Firearm = firearm;
            MaxAmmo = maxAmmo;

            tryReloadDelegate = tryReload;
            tryUnloadDelegate = tryUnload;
            tryStopReloadDelegate = tryStopReload;
        }

        public FirearmItem Firearm { get; }

        public virtual byte MaxAmmo { get; set; }
        public virtual bool Standby { get; set; }

        public virtual bool ServerTryReload()
        {
            if (tryReloadDelegate != null)
                return tryReloadDelegate();

            return true;
        }

        public virtual bool ServerTryStopReload()
        {
            if (tryStopReloadDelegate != null)
                return tryStopReloadDelegate();

            return true;
        }

        public virtual bool ServerTryUnload()
        {
            if (tryUnloadDelegate != null)
                return tryUnloadDelegate();

            return true;
        }
    }
}