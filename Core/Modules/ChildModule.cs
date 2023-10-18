namespace Compendium.Modules
{
    public class ChildModule
    {
        public virtual ChildModuleUpdateOptions UpdateOptions { get; } = new ChildModuleUpdateOptions
        {
            DelayCall = -1,

            UseMainThread = true,
            UseThread = false
        };

        public ParentModule Parent { get; internal set; }

        public virtual void OnStarted() { }
        public virtual void OnStopped() { }
        public virtual void OnDestroyed() { }
        public virtual void OnUpdate() { }

        public TModule GetOrAddModule<TModule>() where TModule : ChildModule, new()
            => Parent.GetOrAddModule<TModule>();

        public TModule GetModule<TModule>() where TModule : ChildModule, new()
            => Parent.GetModule<TModule>();

        public TModule AddModule<TModule>() where TModule : ChildModule, new()
            => Parent.AddModule<TModule>();

        public bool RemoveModule<TModule>() where TModule : ChildModule, new()
            => Parent.RemoveModule<TModule>();
    }
}