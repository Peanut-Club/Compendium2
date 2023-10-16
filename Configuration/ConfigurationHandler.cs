using Compendium.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compendium.Configuration
{
    public static class ConfigurationHandler
    {
        private static readonly List<ConfigurationData> _knownConfigs = new List<ConfigurationData>();

        public static void RegisterAssembly(Assembly assembly, string configId = null)
            => RegisterAttributes(GetAttributes(assembly, configId));

        public static void UnregisterAssembly(Assembly assembly)
            => GetConfigs(assembly).ForEach(key => Unregister(key.Id, key.Key));

        public static void RegisterAttributes(IEnumerable<AttributeInfo<ConfigurationAttribute>> attributes)
        {

        }

        public static bool Unregister(string id, string name)
        {
            var total = 0;

            for (int i = 0; i < _knownConfigs.Count; i++)
            {
                if (_knownConfigs[i].Id == id)
                {
                    total += _knownConfigs[i].Keys.RemoveAll(key => key.Key == name);

                    if (_knownConfigs[i].Keys.Count <= 0)
                    {
                        _knownConfigs.RemoveAt(i);
                        i--;
                    }
                }
            }

            return total > 0;
        }

        public static void CreateData(string id, string path)
        {
            for (int i = 0; i < _knownConfigs.Count; i++)
            {
                if (_knownConfigs[i].Id == id)
                    return;
            }

            _knownConfigs.Add(new ConfigurationData { Id = id, Path = path });
        }

        public static void Register(
            string id, 
            string name,
            string description,
            string path,
            
            Func<object, object> getter,
            Action<object, object> setter,
            
            params Action[] reload)
        {
            ConfigurationData target = null;

            for (int i = 0; i < _knownConfigs.Count; i++)
            {
                if (_knownConfigs[i].Id == id)
                {
                    target = _knownConfigs[i];
                    break;
                }
            }

            if (target is null)
            {
                target = new ConfigurationData { Id = id, Path = path };
                _knownConfigs.Add(target);
            }

            ConfigurationKey key = null;

            for (int i = 0; i < target.Keys.Count; i++)
            {
                if (target.Keys[i].Key == name)
                {
                    key = target.Keys[i];
                    break;
                }
            }

            if (key is null)
            {
                key = new ConfigurationKey { Key = name, Description = description };
                target.Keys.Add(key);
            }

            key.Key = name;
            key.Description = description;

            key.Callbacks.Setter = setter;
            key.Callbacks.Getter = getter;

            foreach (var reloadHandler in reload.Where(act => !target.ReloadHandlers.Contains(act)))
                target.ReloadHandlers.Add(reloadHandler);
        }

        public static void Reload(string id)
        {
            for (int i = 0; i < _knownConfigs.Count; i++)
            {
                if (_knownConfigs[i].Id == id)
                {
                    Reload(_knownConfigs[i]);
                    break;
                }
            }
        }

        public static void ReloadAll()
        {
            for (int i = 0; i < _knownConfigs.Count; i++)
                Reload(_knownConfigs[i]);
        }

        public static void Reload(ConfigurationData configuration)
        {

        }

        public static List<ConfigurationKey> GetConfigs(Assembly assembly)
        {
            var list = new List<ConfigurationKey>();

            for (int i = 0; i < _knownConfigs.Count; i++)
            {
                for (int x = 0; x < _knownConfigs[i].Keys.Count; x++)
                {
                    if (_knownConfigs[i].Keys[x].Assembly != null && _knownConfigs[i].Keys[x].Assembly == assembly)
                        list.Add(_knownConfigs[i].Keys[x]);
                }
            }

            return list;
        }

        public static List<AttributeInfo<ConfigurationAttribute>> GetAttributes(Assembly assembly, string assignId = null)
        {
            var attributes = new List<AttributeInfo<ConfigurationAttribute>>();

            assembly
                .GetTypes()
                .ForEach(type => attributes.AddRange(AttributeResolver<ConfigurationAttribute>.ResolveAttributes(type, null, AttributeResolveFlags.Field | AttributeResolveFlags.Property)));

            if (!string.IsNullOrWhiteSpace(assignId))
            {
                for (int i = 0; i < attributes.Count; i++)
                    attributes[i].Attribute!.Id = assignId;
            }

            return attributes;
        }
    }
}