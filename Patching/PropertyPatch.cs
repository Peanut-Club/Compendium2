using Compendium.Extensions;

using System.Reflection;

namespace Compendium.Patching
{
    public class PropertyPatch
    {
        public PropertyInfo Property { get; }
        public DetourHandler Detour { get; }

        public bool IsApplied { get => Detour.IsApplied; set => Detour.IsApplied = value; }

        public object Value { get => Property.GetValue(Handle); set => Property.SetValue(Handle, value); }
        public object Handle { get; set; }

        public PropertyPatch(PropertyInfo property, bool isGetter, MethodBase patch)
        {
            Property = property;
            Detour = (isGetter ? property.GetGetMethod(true) : property.GetSetMethod(true)).CreateAndApplyDetour(patch);
        }
    }
}