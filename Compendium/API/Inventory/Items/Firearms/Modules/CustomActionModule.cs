using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;

using System;

namespace Compendium.API.Inventory.Items.Firearms.Modules
{
    public class CustomActionModule : IActionModule
    {
        private Func<bool> authorizeDryFireDelegate;
        private Func<bool> authorizeShotDelegate;

        public virtual float CyclicRate { get; set; }

        public virtual bool IsTriggerHeld { get; set; }
        public virtual bool Standby { get; set; }

        public virtual FirearmStatus PredictedStatus { get; set; }

        public CustomActionModule(Func<bool> authorizeDryFire = null, Func<bool> authorizeShot = null)
        {
            authorizeDryFireDelegate = authorizeDryFire;
            authorizeShotDelegate = authorizeShot;
        }

        public bool ServerAuthorizeDryFire()
        {
            if (authorizeDryFireDelegate != null)
                return authorizeDryFireDelegate();

            return true;
        }

        public bool ServerAuthorizeShot()
        {
            if (authorizeShotDelegate != null)
                return authorizeShotDelegate();

            return true;
        }


        public ActionModuleResponse DoClientsideAction(bool isTriggerPressed)
            => ActionModuleResponse.Idle;
    }
}