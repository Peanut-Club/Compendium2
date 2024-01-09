using Common.Extensions;

using System;
using System.Collections.Generic;

namespace Compendium.Ipc
{
    public static class IpcManager
    {
        private static readonly Dictionary<Type, List<Type>> ipcObjectBinds = new Dictionary<Type, List<Type>>();
        private static readonly Dictionary<object, List<IpcMethod>> ipcMethodBinds = new Dictionary<object, List<IpcMethod>>();

        public static void BindIpcType<TType>(string targetTypeName) where TType : IpcObject, new()
        {
            if (!ipcObjectBinds.TryGetFirst(type => type.Key.FullName == targetTypeName, out var pair)
                || pair.Value is null || pair.Value.Contains(typeof(TType)))
                return;

            pair.Value.Add(typeof(TType));
        }
    }
}