using System.Collections.Generic;

namespace Compendium.Modules
{
    public class ParentModule
    {
        private readonly List<ChildModule> _children = new List<ChildModule>();

        public void Update()
        {

        }

        public TModule GetOrAddModule<TModule>() where TModule : ChildModule, new()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TModule module)
                    return module;
            }

            return AddModule<TModule>();
        }

        public TModule GetModule<TModule>() where TModule : ChildModule, new() 
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TModule module)
                    return module;
            }

            return default;
        }

        public TModule AddModule<TModule>() where TModule : ChildModule, new()
        {
            var module = GetModule<TModule>();

            if (module != null)
                return module;

            module = new TModule();

            module.Parent = this;
            module.OnStarted();

            _children.Add(module);

            return module;
        }

        public bool RemoveModule<TModule>() where TModule : ChildModule, new()
        {
            var module = GetModule<TModule>();

            if (module is null)
                return false;

            module.OnStopped();
            module.OnDestroyed();

            module.Parent = null;

            return _children.RemoveAll(m => m is TModule) > 0;
        }
    }
}