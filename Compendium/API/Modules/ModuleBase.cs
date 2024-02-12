using Common.Logging;

namespace Compendium.API.Modules
{
    public class ModuleBase
    {
        public virtual string Name { get; }

        public bool IsActive { get; internal set; }
        public bool IsUpdateActive { get; internal set; }

        public ModuleUpdate Update { get; internal set; }
        public ModuleManager Manager { get; internal set; }

        public LogOutput Log { get; internal set; }

        public virtual ModuleUpdate OnStart()
            => null;

        public virtual bool IsValid()
            => IsActive;

        public virtual void OnStop() { }
    }
}