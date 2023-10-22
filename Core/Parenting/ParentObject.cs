using Compendium.Pooling.Pools;
using Compendium.Utilities;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Parenting
{
    public class ParentObject
    {
        private List<ChildObject> _children;

        public IReadOnlyList<ChildObject> Children => _children;

        public bool AllowsMultipleOfType { get; set; } = true;

        public Type[] LimitsToTypes { get; set; }

        public ParentObject()
        {
            _children = new List<ChildObject>();
        }

        public TChildren[] GetOfType<TChildren>()
            => _children.Where(c => c is TChildren).CastArray<TChildren>();

        public TChildren[] GetOfType<TChildren>(Predicate<TChildren> predicate)
        {
            var tempList = ListPool<TChildren>.Shared.Rent();

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TChildren children
                    && predicate.SafeCall(children))
                    tempList.Add(children);
            }

            return ListPool<TChildren>.Shared.ToArrayReturn(tempList);
        }

        public TChildren GetFirstOfType<TChildren>() where TChildren : ChildObject, new()
            => (TChildren)_children.FirstOrDefault(c => c is TChildren);

        public TChildren GetFirstOfType<TChildren>(Predicate<TChildren> predicate) where TChildren : ChildObject, new()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TChildren children
                    && predicate.SafeCall(children))
                    return children;
            }

            return default;
        }

        public void AddChild<TChildren>() where TChildren : ChildObject, new()
            => AddInstance(new TChildren());

        public void RemoveChild<TChildren>() where TChildren : ChildObject, new()
            => _children.RemoveAll(c => c is TChildren);

        public void AddInstances(params ChildObject[] childObjects)
            => childObjects.ForEach(AddInstance);

        public void AddInstance(ChildObject childObject)
        {
            if (childObject is null)
                throw new ArgumentNullException(nameof(childObject));

            if (!AllowsMultipleOfType && _children.Any(c => c.GetType() == childObject.GetType()))
                throw new ArgumentException($"This parent object limits the count of '{childObject.GetType().ToName()}' type to one.");

            if (LimitsToTypes != null && LimitsToTypes.Length > 0 && !LimitsToTypes.Contains(childObject.GetType()))
                throw new ArgumentException($"This parent object does not accept children of type '{childObject.GetType().ToName()}'");

            if (childObject.Parent != null)
                childObject.Parent.RemoveInstance(childObject);

            _children.Add(childObject);

            childObject.Parent = this;
            childObject.OnAddedToParent();
        }

        public void RemoveInstance(ChildObject childObject)
        {
            if (childObject is null)
                throw new ArgumentNullException(nameof(childObject));

            _children.Remove(childObject);

            childObject.Parent = null;
            childObject.OnRemovedFromParent();
        }
    }
}