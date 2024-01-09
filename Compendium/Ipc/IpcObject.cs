using System.Collections.Generic;
using System.Reflection;

namespace Compendium.Ipc
{
    public class IpcObject
    {
        internal readonly List<FieldInfo> ipcFields = new List<FieldInfo>();
        internal readonly List<MethodInfo> ipcMethods = new List<MethodInfo>();
        internal readonly List<PropertyInfo> ipcProperties = new List<PropertyInfo>();
    }
}