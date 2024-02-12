using Common.IO.Collections;
using Common.Extensions;
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
        
        public LogOutput Log { get; private set; }

        public ModuleManager()
        {
            modules = new LockedDictionary<Type, ModuleBase>();
            modulesLock = new object();

            Log = new LogOutput("Module Manager").Setup();

            IsValid = true;

            CodeUtils.WhileTrue(() => IsValid, UpdateModules, 15);
        }

        public T Add<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                if (modules.TryGetValue(typeof(T), out var module)
                    && module.IsActive)
                    return (T)module;

                try
                {
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
                catch (Exception ex)
                {
                    Log.Error($"An error occured while adding module '{typeof(T).FullName}':\n{ex}");
                    return default;
                }
            }
        }

        public T Get<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                if (modules.TryGetValue(typeof(T), out var module)
                    && module.IsValid())
                    return (T)module;

                return default;
            }
        }

        public bool Destroy<T>() where T : ModuleBase
        {
            lock (modulesLock)
            {
                try
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
                catch (Exception ex)
                {
                    Log.Error($"An error has occured while destroying module '{typeof(T).FullName}':\n{ex}");
                    return false;
                }
            }
        }

        public void Destroy()
        {
            if (!IsValid)
                throw new InvalidOperationException($"This module manager has already been destroyed!");

            lock (modulesLock)
            {
                foreach (var module in modules.Values)
                {
                    try
                    {
                        module.IsActive = false;
                        module.IsUpdateActive = false;

                        module.Update = null;

                        module.OnStop();

                        module.Log?.Dispose();
                        module.Log = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"An error has occured while destroying module '{module.Name}':\n{ex}");
                    }
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
                    if (module.Value != null && module.Value.IsValid() && module.Value.IsUpdateActive)
                    {
                        if (DateTime.Now >= module.Value.Update.NextCallTime)
                        {
                            module.Value.Update.UpdateMethod.Call(module.Value, Log.Error);
                            module.Value.Update.LastCallTime = DateTime.Now;

                            var nextInterval = module.Value.Update.Interval.Call(Log.Error);

                            if (nextInterval > 0)
                                module.Value.Update.NextCallTime = DateTime.Now.AddMilliseconds(nextInterval);
                            else
                                module.Value.Update.NextCallTime = DateTime.MaxValue;
                        }
                    }
                }
            }
        }
    }
}