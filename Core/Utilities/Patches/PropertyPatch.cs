using Compendium.Extensions;
using Compendium.Patching.Detours;
using Compendium.Utilities.Reflection;

using System;
using System.Reflection;

namespace Compendium.Utilities.Patches
{
    public class PropertyPatch
    {
        public PropertyInfo Property { get; }
        public DetourHandler Detour { get; }

        public bool IsApplied { get => Detour.IsApplied; set => Detour.IsApplied = value; }

        public object Value { get => Property.GetValueFast<object>(Handle); set => Property.SetValueFast(Handle, value); }
        public object Handle { get; set; }

        public PropertyPatch(PropertyInfo property, bool isGetter, MethodBase patch)
        {
            Property = property;
            Detour = (isGetter ? property.GetGetMethod(true) : property.GetSetMethod(true)).CreateAndApplyDetour(patch);
        }

        public PropertyPatch(PropertyInfo property, Func<object, object> getter)
            : this(property, true, getter.GetMethodInfo()) { }

        public PropertyPatch(PropertyInfo property, Action<object, object> setter) 
            : this(property, false, setter.GetMethodInfo()) { }
    }
}