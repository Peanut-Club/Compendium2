using Compendium.Pooling.Pools;
using Compendium.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace Compendium.Components
{
    public class ComponentContainer
    {
        public GameObject Target { get; }

        public IReadOnlyDictionary<Type, Component> Components { get; }

        public ComponentContainer(GameObject target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            Target = target;

            var comps = target.GetAllComponents();
            var dict = DictionaryPool<Type, Component>.Shared.Rent();

            for (int i = 0; i < comps.Length; i++)
                dict[comps[i].GetType()] = comps[i];

            Components = new ReadOnlyDictionary<Type, Component>(dict);

            DictionaryPool<Type, Component>.Shared.Return(dict);
        }

        public TComponent GetComponent<TComponent>() where TComponent : Component
            => TryGetComponent<TComponent>(out var comp) ? comp : null;

        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component
            => ((Components.TryGetValue(typeof(TComponent), out var comp) && comp is TComponent tComp) ? (component = tComp) : (component = null)) != null;
    }
}