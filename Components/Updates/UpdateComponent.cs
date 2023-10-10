using System;
using System.Collections.Generic;

using Compendium.Utilities.Reflection;

using MEC;

namespace Compendium.Components.Updates
{
    public class UpdateComponent : Component
    {
        public static UpdateComponent ParentUpdate { get; } = new UpdateComponent(true);

        private List<UpdateInfo> _updates = new List<UpdateInfo>();

        public UpdateComponent(bool isParent = false)
        {
            if (!isParent)
                Parent = ParentUpdate;
        }

        public UpdateInfo Create(Action target, UpdateConfiguration config = null)
        {
            for (int i = 0; i < _updates.Count; i++)
            {
                if (_updates[i].Target.Method == target.Method && ObjectUtilities.IsInstance(target.Target, _updates[i].Target.Target))
                    return _updates[i];
            }

            var update = new UpdateInfo(target, config ?? new UpdateConfiguration());

            _updates.Add(update);

            return update;
        }

        public void Remove(UpdateInfo update)
            => _updates.Remove(update);

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < _updates.Count; i++)
            {
                if (_updates[i].Configuration.Segment is Segment.Update)
                    _updates[i].Call();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            for (int i = 0; i < _updates.Count; i++)
            {
                if (_updates[i].Configuration.Segment is Segment.FixedUpdate)
                    _updates[i].Call();
            }
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            for (int i = 0; i < _updates.Count; i++)
            {
                if (_updates[i].Configuration.Segment == Segment.LateUpdate)
                    _updates[i].Call();
            }
        }
    }
}