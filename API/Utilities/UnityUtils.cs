using UnityEngine;

namespace Compendium.Utilities
{
    public static class UnityUtils
    {
        public static Component[] GetAllComponents(this GameObject gameObject)
            => gameObject.GetComponents<Component>();

        public static Component[] GetAllComponents(this Component component)
            => component.GetComponents<Component>();
    }
}