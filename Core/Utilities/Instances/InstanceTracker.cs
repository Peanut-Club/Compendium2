using Compendium.Attributes;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Compendium.Utilities.Instances
{
    public static class InstanceTracker
    {
        private static readonly Dictionary<Type, object> _trackedInstances = new Dictionary<Type, object>();

        private static readonly Dictionary<Type, List<InstanceCreator>> _cachedConstructors = new Dictionary<Type, List<InstanceCreator>>();
        private static readonly Dictionary<Type, List<AttributeInfo<InstanceAttribute>>> _cachedAttributes = new Dictionary<Type, List<AttributeInfo<InstanceAttribute>>>();

        public static event Action<Type, object> OnInstanceCreated;

        public static void Set(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            _trackedInstances[type] = instance;

            OnInstanceCreated.SafeCall(type, instance);

            if (!_cachedAttributes.ContainsKey(type))
                FetchAttributes(type);

            if (_cachedAttributes.TryGetValue(type, out var attrs) && attrs != null && attrs.Count > 0)
            {
                for (int i = 0; i < attrs.Count; i++)
                {
                    if (attrs[i].Property != null 
                        && attrs[i].Property.SetMethod != null 
                        && attrs[i].Property.SetMethod.IsStatic
                        && attrs[i].Property.PropertyType == type)
                        attrs[i].Property.SetValue(null, instance);

                    if (attrs[i].Field != null
                        && attrs[i].Field.IsStatic
                        && attrs[i].Field.FieldType == type)
                        attrs[i].Field.SetValue(null, instance);
                }
            }
        }

        public static T Get<T>(object[] args = null, bool createIfMissing = false)
        {
            var instance = Get(typeof(T), args, createIfMissing);

            if (instance is null)
                return default;

            if (instance is not T t)
                throw new InvalidCastException($"Cannot instantiate type {instance.GetType().FullName} as {typeof(T).FullName}");

            return t;
        }

        public static object Get(Type type, object[] args = null, bool createIfMissing = false)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (_trackedInstances.TryGetValue(type, out var instance))
                return instance;

            if (!createIfMissing)
                throw new ArgumentException($"Type '{type.FullName}' was not found in the instance cache.");

            if (!_cachedConstructors.ContainsKey(type))
                BuildConstructors(type);

            var creator = GetBestCreator(type, args);

            if (creator is null)
                throw new InvalidOperationException($"Type '{type.FullName}' does not have any available instance creators.");

            instance = creator.Invoke(args);

            if (instance is null)
                throw new InvalidOperationException($"Instance Creator failed to instantiate type '{type.FullName}'.");

            Set(instance);

            return instance;
        }

        private static InstanceCreator GetBestCreator(Type type, object[] args)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!_cachedConstructors.TryGetValue(type, out var creators) || creators.Count < 1)
                return null;

            if (args is null || args.Length < 1)
                return creators.First(c => c.Types is null || c.Types.Length < 1);

            for (int i = 0; i < creators.Count; i++)
            {
                if (creators[i].Types is null || creators[i].Types.Length != args.Length)
                    continue;

                return creators[i];
            }

            return null;
        }

        private static void FetchAttributes(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            _cachedAttributes[type] = AttributeResolver<InstanceAttribute>.ResolveAttributes(type, AttributeResolveFlags.Field | AttributeResolveFlags.Property);
        }

        private static void BuildConstructors(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (_cachedConstructors.ContainsKey(type))
                return;

            var constructors = type.GetAllConstructors();

            if (constructors is null || constructors.Length < 1)
                throw new InvalidOperationException($"Type '{type.FullName}' has no available constructors!");

            _cachedConstructors[type] = new List<InstanceCreator>();

            for (int i = 0; i < constructors.Length; i++)
                _cachedConstructors[type].Add(new InstanceCreator(constructors[i]));
        }
    }
}