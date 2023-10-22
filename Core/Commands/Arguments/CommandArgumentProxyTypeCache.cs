using System;
using System.Collections.Generic;

namespace Compendium.Commands.Arguments
{
    public class CommandArgumentProxyTypeCache
    {
        private readonly Dictionary<Type, Type> _proxyTypeCache = new Dictionary<Type, Type>();
        private readonly List<Type> _clearTypes = new List<Type>();

        public bool IsContained(ref Type type)
        {
            if (_clearTypes.Contains(type))
                return true;

            if (_proxyTypeCache.ContainsKey(type))
            {
                type = _proxyTypeCache[type];
                return true;
            }

            return false;
        }

        public void Add(Type type, Type proxy)
        {
            if (proxy is null)
                _clearTypes.Add(type);
            else
                _proxyTypeCache[type] = proxy;
        }
    }
}
