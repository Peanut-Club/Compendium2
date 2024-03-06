using Compendium.API.Core;

using InventorySystem.Items.Firearms.Modules;

namespace Compendium.API.Inventory.Items.Firearms.Modules
{
    public class WrappedAmmoManager : Wrapper<IAmmoManagerModule>, IAmmoManagerModule
    {
        public WrappedAmmoManager(IAmmoManagerModule baseValue) : base(baseValue)
        {
            MaxAmmo = baseValue.MaxAmmo;
        }

        public virtual byte MaxAmmo { get; set; }

        public virtual bool Standby
        {
            get => Base.Standby;
        }

        public virtual bool ServerTryReload()
            => Base.ServerTryReload();

        public virtual bool ServerTryStopReload()
            => Base.ServerTryStopReload();

        public virtual bool ServerTryUnload()
            => Base.ServerTryUnload();
    }
}