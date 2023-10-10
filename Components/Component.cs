using Compendium.Utilities;

using MEC;

using System.Collections.Generic;

namespace Compendium.Components
{
    public class Component : Disposable
    {
        private List<Component> _childComponents = new List<Component>();

        private CoroutineHandle _update;
        private CoroutineHandle _fixed;
        private CoroutineHandle _late;

        public Component Parent { get; set; }

        public IReadOnlyList<Component> Children => _childComponents;

        public TComponent Add<TComponent>() where TComponent : Component, new()
        {
            var comp = new TComponent();
            comp.Start(this);
            _childComponents.Add(comp);
            return comp;
        }

        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component
        {
            for (int i = 0; i < _childComponents.Count; i++)
            {
                if (Parent != null && Parent is TComponent)
                {
                    component = Parent as TComponent;
                    return true;
                }

                if (_childComponents[i] is TComponent)
                {
                    component = _childComponents[i] as TComponent;
                    return true;
                }

                if (_childComponents[i].TryGetComponent(out component))
                    return true;
            }

            component = default;
            return false;
        }

        public virtual void OnStart() { }

        public virtual void OnUpdate() 
        { 
            
        }

        public virtual void OnLateUpdate() 
        { 

        }

        public virtual void OnFixedUpdate() 
        { 

        }
       
        public virtual void OnDestroy() { }

        public override void DisposeInternal()
        {
            Timing.KillCoroutines(_update, _late, _fixed);

            OnDestroy();

            base.DisposeInternal();
        }

        internal void Start(Component parent)
        {
            Parent = parent;

            OnStart();

            if (Parent is null)
            {
                _update = Timing.RunCoroutine(Update(), Segment.Update);
                _late = Timing.RunCoroutine(Late(), Segment.LateUpdate);
                _fixed = Timing.RunCoroutine(Fixed(), Segment.FixedUpdate);
            }
        }

        private IEnumerator<float> Update()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                OnUpdate();

                for (int i = 0; i < _childComponents.Count; i++)
                    _childComponents[i].OnUpdate();
            }
        }

        private IEnumerator<float> Late()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                OnLateUpdate();

                for (int i = 0; i < _childComponents.Count; i++)
                    _childComponents[i].OnLateUpdate();
            }
        }

        private IEnumerator<float> Fixed()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                OnFixedUpdate();

                for (int i = 0; i < _childComponents.Count; i++)
                    _childComponents[i].OnFixedUpdate();
            }
        }

        public static TComponent Create<TComponent>(Component parent = null) where TComponent : Component, new()
        {
            var comp = new TComponent();

            comp.Start(parent);

            return comp;
        }

        public static void Destroy(Component component)
        {
            component.Dispose();

            foreach (var child in component.Children)
                child.Dispose();

            if (component.Parent != null && !component.Parent.IsDisposing && !component.Parent.IsDisposed)
                component.Parent.Dispose();
        }
    }
}