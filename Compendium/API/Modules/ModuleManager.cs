using Common.Extensions;
using Common.IO.Collections;
using Common.Utilities;
using Common.Logging;

using System;

namespace Compendium.API.Modules
{
    public class ModuleManager
    {
        private LockedDictionary<Type, ModuleBase> modules;
        private object modulesLock;

        public bool IsValid { get; private set; }

        public ModuleManager()
        {
            modules = new LockedDictionary<Type, ModuleBase>();
            modulesLock = new object();

            CodeUtils.WhileTrue(() => IsValid, UpdateModules, 5);
        }

        public T Add<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                if (modules.TryGetValue(typeof(T), out var module)
                    && module.IsActive)
                    return (T)module;

                module = typeof(T).Construct<T>();

                module.Manager = this;
                module.Log = new LogOutput(typeof(T).Name.SpaceByUpperCase()).Setup();

                var moduleUpdate = module.OnStart();

                if (moduleUpdate != null && moduleUpdate.UpdateMethod != null)
                {
                    module.IsUpdateActive = true;
                    module.Update = moduleUpdate;
                }

                return (T)module;
            }
        }

        public T Get<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                if (modules.TryGetValue(typeof(T), out var module)
                    && module.IsActive)
                    return (T)module;

                return null;
            }
        }

        public bool Destroy<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                if (modules.TryGetValue(typeof(T), out var module))
                {
                    module.IsActive = false;
                    module.IsUpdateActive = false;
                    module.Update = null;

                    module.OnStop();

                    module.Log?.Dispose();
                    module.Log = null;
                }

                return modules.Remove(typeof(T));
            }
        }

        public void Destroy()
        {
            lock (modulesLock)
            {
                foreach (var module in modules)
                {
                    module.Value.IsActive = false;
                    module.Value.IsUpdateActive = false;
                    module.Value.Update = null;

                    module.Value.OnStop();

                    module.Value.Log?.Dispose();
                    module.Value.Log = null;
                }

                modules.Clear();
            }

            IsValid = false;

            modulesLock = null;
            modules = null;
        }

        private void UpdateModules()
        {
            lock (modulesLock)
            {
                foreach (var module in modules)
                {
                    if (module.Value != null && module.Value.IsActive && module.Value.IsUpdateActive && module.Value.Update != null)
                    {
                        if (DateTime.Now > module.Value.Update.NextCallTime)
                        {
                            module.Value.Update.UpdateMethod.Call(module.Value);
                            module.Value.Update.LastCallTime = DateTime.Now;
                            module.Value.Update.NextCallTime = module.Value.Update.LastCallTime.AddMilliseconds(module.Value.Update.Interval());
                        }
                    }
                }
            }
        }
    }
}