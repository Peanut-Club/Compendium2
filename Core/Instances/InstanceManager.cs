using Compendium.Logging;
using Compendium.Utilities.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Instances
{
    public static class InstanceManager
    {
        private static readonly Dictionary<Type, Func<object>> _compiledConstructors = new Dictionary<Type, Func<object>>();
        private static readonly Dictionary<Type, List<MemberInfo>> _instantiationListeners = new Dictionary<Type, List<MemberInfo>>();

        private static readonly List<InstanceDescriptor> _descriptors = new List<InstanceDescriptor>();

        public static event Action<InstanceDescriptor> OnCreated;

        public static void ScanAndInstantiate()
            => ScanAndInstantiate(Assembly.GetCallingAssembly());

        public static void ScanAndInstantiate(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                ScanAndInstantiate(type);
        }

        public static void ScanAndInstantiate(Type type)
        {
            if (!type.IsStatic() && type.HasAttribute<InstantiateAttribute>())
            {
                var constructor = InstanceUtils.FindConstructor(type);

                if (constructor != null)
                {
                    var lambda = InstanceUtils.CompileConstructor(constructor);

                    if (lambda != null)
                        _compiledConstructors[type] = lambda;
                    else
                        Log.Error("Instance Manager", $"Failed to compile a constructing lambda expression for type '{type.ToName()}'");
                }
                else
                    Log.Error("Instance Manager", $"Type '{type.ToName()}' is marked with the 'Instantiate' attribute, but it doesn't have any parameter-less constructors.");
            }

            _instantiationListeners[type] = new List<MemberInfo>();

            var fields = type.GetAllFields();
            var props = type.GetAllProperties();

            foreach (var field in fields)
            {
                if (!field.HasAttribute<InstanceAttribute>(out var instanceAttribute))
                    continue;

                if (instanceAttribute.Types.Length > 0 && !instanceAttribute.Types.Contains(type))
                    continue;

                if (!field.IsStatic || field.IsInitOnly)
                {
                    Log.Error("Instance Manager", $"Field '{field.ToName()}' is marked as an Instance listener, but it is not static or writable.");
                    continue;
                }

                if (field.FieldType != type && !type.InheritsType(field.FieldType))
                    continue;

                _instantiationListeners[type].Add(field);
            }

            foreach (var prop in props)
            {
                if (!prop.HasAttribute<InstanceAttribute>(out var instanceAttribute))
                    continue;

                if (instanceAttribute.Types.Length > 0 && !instanceAttribute.Types.Contains(type))
                    continue;

                if (!prop.CanWrite || prop.SetMethod is null || !prop.SetMethod.IsStatic)
                {
                    Log.Error("Instance Manager", $"Property '{prop.ToName()}' is marked as an Instance listener, but it is not static or writable.");
                    continue;
                }

                if ((prop.PropertyType != type && !type.InheritsType(prop.PropertyType)))
                    continue;

                _instantiationListeners[type].Add(prop);
            }

            Instantiate(type);
        }

        public static void Instantiate(Type type)
        {
            if (!_compiledConstructors.TryGetValue(type, out var constructor))
                return;

            if (_descriptors.Any(d => d.Type == type && d.Reference.IsAlive))
                return;

            var value = constructor.SafeCall();

            if (value is null)
                return;

            if (_instantiationListeners.TryGetValue(type, out var members))
            {
                foreach (var member in members)
                {
                    if (member is FieldInfo field)
                        field.SetValue(null, value);
                    else if (member is PropertyInfo prop)
                        prop.SetValue(null, value);
                }
            }

            var descriptor = new InstanceDescriptor(value);

            _descriptors.RemoveAll(d => d.Type == type);
            _descriptors.Add(descriptor);

            OnCreated.SafeCall(descriptor);
        }

        public static object Get(Type type, bool addIfMissingOrDead = true)
        {
            for (int i = 0; i < _descriptors.Count; i++)
            {
                if (_descriptors[i].Type == type && _descriptors[i].Reference.IsAlive)
                    return _descriptors[i].Reference.Value;
            }

            if (addIfMissingOrDead)
            {
                if (!_compiledConstructors.ContainsKey(type))
                    ScanAndInstantiate(type);
                else
                    Instantiate(type);

                return Get(type, false);
            }

            return null;
        }
    }
}