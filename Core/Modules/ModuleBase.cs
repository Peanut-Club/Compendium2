using Compendium.Pooling.Pools;
using Compendium.Utilities.Reflection;

using MEC;

using System;
using System.Collections.Generic;

namespace Compendium.Modules
{
    public class ModuleBase
    {
        private static readonly List<ModuleBase> _allModules = new List<ModuleBase>();

        public static IReadOnlyList<ModuleBase> AllModules => _allModules;

        private HashSet<ModuleBase> _childModules = new HashSet<ModuleBase>();
        private CoroutineHandle _coroutine;

        public IReadOnlyCollection<ModuleBase> Children => _childModules;

        public ModuleBase ParentModule { get; private set; }
        public ModuleUpdateInfo UpdateInfo { get; private set; }

        public ModuleStatus Status { get; private set; }

        public ModuleBase() 
            => _allModules.Add(this);

        public TModule GetOrAddModule<TModule>(bool includeParent = true) where TModule : ModuleBase, new()
        {
            var foundModule = GetModule<TModule>(includeParent);

            if (foundModule != null)
                return foundModule;

            return AddModule<TModule>();
        }

        public TModule GetModule<TModule>(bool includeParent = true) where TModule : ModuleBase
        {
            if (includeParent && ParentModule != null && ParentModule is TModule module)
                return module;

            foreach (var child in _childModules)
            {
                if (child is TModule childModule)
                    return childModule;
            }

            return default;
        }

        public TModule[] GetModules<TModule>(bool includeParent = true) where TModule : ModuleBase
        {
            var list = ListPool<TModule>.Shared.Rent();

            if (includeParent && ParentModule != null && ParentModule is TModule parentModule)
                list.Add(parentModule);

            foreach (var child in _childModules)
            {
                if (child is TModule childModule)
                    list.Add(childModule);
            }

            return ListPool<TModule>.Shared.ToArrayReturn(list);
        }

        public TModule AddModule<TModule>() where TModule : ModuleBase, new()
        {
            var module = new TModule();

            module.Setup(this);

            _childModules.Add(module);

            return module;
        }

        public void RemoveModules<TModule>() where TModule : ModuleBase
        {
            var modules = GetModules<TModule>();

            for (int i = 0; i < modules.Length; i++)
                modules[i].Destroy();

            _childModules.RemoveWhere(m => m is TModule);
        }

        public virtual void OnUpdate() { }

        public virtual void OnStarted() { }
        public virtual void OnDestroyed() { }

        public virtual void OnParentChanged() { }
        public virtual void OnParentAdded() { }
        public virtual void OnParentRemoved() { }

        public virtual void OnChildRemoved(ModuleBase child) { }
        public virtual void OnChildAdded(ModuleBase child) { }
        public virtual void OnChildDestroyed(ModuleBase child) { }

        public void AttachChild(ModuleBase child)
        {
            if (_childModules.Contains(child))
                return;

            _childModules.Add(child);

            OnChildAdded(child);
        }

        public void DestroyChild(ModuleBase child)
        {
            if (_childModules.TryGetValue(child, out child))
            {
                child.Destroy();
                _childModules.Remove(child);
            }
        }

        public void RemoveChild(ModuleBase child)
        {
            if (_childModules.Remove(child))
            {
                child.ChangeParent(null, false);
                OnChildRemoved(child);
            }
        }

        internal void Destroy()
        {
            Status = ModuleStatus.Destroying;

            ChangeParent(null, true);
            OnDestroyed();

            Status = ModuleStatus.Destroyed;

            foreach (var child in _childModules)
                child.Destroy();

            _childModules.Clear();

            _allModules.Remove(this);
        }

        internal void ChangeParent(ModuleBase newParent, bool isDestroying)
        {
            if (isDestroying)
            {
                if (ParentModule != null)
                {
                    ParentModule.OnChildDestroyed(this);
                    ParentModule.OnChildRemoved(this);
                    ParentModule = null;

                    OnParentRemoved();
                }
            }
            else if (ParentModule != null && newParent != null && ParentModule == newParent) { }
            else if (newParent != null)
            {
                if (ParentModule != null)
                {
                    ParentModule.OnChildRemoved(this);
                    ParentModule = newParent;
                    ParentModule.OnChildAdded(this);

                    OnParentChanged();
                }
                else
                {
                    ParentModule = newParent;
                    ParentModule.OnChildAdded(this);

                    OnParentAdded();
                }
            }
            else
            {
                if (ParentModule != null)
                {
                    ParentModule.OnChildRemoved(this);
                    ParentModule = null;

                    OnParentRemoved();
                }
            }
        }

        internal void Setup(ModuleBase parent)
        {
            Status = ModuleStatus.Starting;

            ChangeParent(parent, false);

            var type = GetType();
            var updateMethod = type.GetMethod("OnUpdate");

            if (updateMethod is null || updateMethod.DeclaringType == typeof(ModuleBase))
                return;

            if (updateMethod.HasAttribute<ModuleUpdateAttribute>(out var moduleUpdateAttribute))
                UpdateInfo = new ModuleUpdateInfo(moduleUpdateAttribute.Delay, updateMethod, moduleUpdateAttribute.Source, moduleUpdateAttribute.IsInherited) { IsProfiled = moduleUpdateAttribute.IsProfiled };
            else
                UpdateInfo = new ModuleUpdateInfo(null, updateMethod, ModuleUpdateSource.UnityEngine, ParentModule != null);

            Status = ModuleStatus.Alive;

            OnStarted();

            if (!UpdateInfo.IsInherited)
                _coroutine = Timing.RunCoroutine(UpdateCoroutine(), Segment.Update);
        }

        internal void ProcessUpdate(bool fromParent)
        {
            if (UpdateInfo is null)
                return;

            if (UpdateInfo.IsInherited && !fromParent)
                return;

            if (UpdateInfo.Delay != null && UpdateInfo.Delay.Value > 0
                && (DateTime.Now - UpdateInfo.LastCalled).TotalMilliseconds < UpdateInfo.Delay.Value)
                return;

            if (UpdateInfo.IsProfiled && UpdateInfo.Profiler.TryNewFrame(out var frame))
            {
                try
                {
                    UpdateInfo.Caller(this, null);
                    UpdateInfo.Profiler.EndFrame(frame, null);
                }
                catch (Exception ex)
                {
                    UpdateInfo.Profiler.EndFrame(frame, ex);
                }
            }
            else
            {
                UpdateInfo.Caller.SafeCall(this, null);
            }

            UpdateInfo.LastCalled = DateTime.Now;
        }

        internal IEnumerator<float> UpdateCoroutine()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;
                yield return Timing.WaitForOneFrame;

                ProcessUpdate(false);

                foreach (var child in _childModules)
                    child.ProcessUpdate(true);
            }
        }
    }
}