using UnityEngine;

using System;

namespace Compendium.Utilities
{
    /// <summary>
    /// Utilities related to Unity Engine.
    /// </summary>
    public static class UnityUtils
    {
        /// <summary>
        /// Retrieves all components of a specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to retrieve components of.</param>
        /// <returns>An <see cref="Array"/> containing all components of this <see cref="GameObject"/>.</returns>
        public static Component[] GetAllComponents(this GameObject gameObject)
            => gameObject.GetComponents<Component>();

        /// <summary>
        /// Retrieves all components of a specified <see cref="Component"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="Component"/> to retrieve components of.</param>
        /// <returns>An <see cref="Array"/> containing all components of this <see cref="Component"/>.</returns>
        public static Component[] GetAllComponents(this Component component)
            => component.GetComponents<Component>();
    }
}