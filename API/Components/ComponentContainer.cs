using Compendium.Pooling.Pools;
using Compendium.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace Compendium.Components
{
    /// <summary>
    /// Used for caching components of a specific <see cref="GameObject"/>.
    /// </summary>
    public class ComponentContainer : Disposable
    {
        /// <summary>
        /// Gets the <see cref="GameObject"/> this instance was initialized with.
        /// </summary>
        public GameObject Target { get; private set; }

        /// <summary>
        /// Gets an <see cref="IReadOnlyDictionary{TKey, TValue}"/> of all components present when initializing this instance.
        /// </summary>
        public IReadOnlyDictionary<Type, Component> Components { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="ComponentContainer"/> instance.
        /// </summary>
        /// <param name="target">The <see cref="GameObject"/> instance to collect components of.</param>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>
        /// Retrieves a component of the specified type from the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type to retrieve.</typeparam>
        /// <returns>The component instance, if found. Otherwise, <see cref="null"/>.</returns>
        public TComponent GetComponent<TComponent>() where TComponent : Component
            => TryGetComponent<TComponent>(out var comp) ? comp : null;

        /// <summary>
        /// Attempts to retrieve a component of the specified type from the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type to retrieve.</typeparam>
        /// <param name="component">The component instance, if found. Otherwise, null.</param>
        /// <returns><see cref="true"/> if the component was found, otherwise <see cref="false"/>.</returns>
        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : Component
            => ((Components.TryGetValue(typeof(TComponent), out var comp) && comp is TComponent tComp) ? (component = tComp) : (component = null)) != null;

        /// <summary>
        /// Cleans up the container.
        /// </summary>
        public override void DisposeInternal()
        {
            base.DisposeInternal();

            Target = null;
            Components = null;
        }
    }
}