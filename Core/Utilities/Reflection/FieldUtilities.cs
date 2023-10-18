using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compendium.Utilities.Reflection
{
    public static class FieldUtilities
    {
        public static FieldInfo[] GetAllFields(this Type type)
            => type.GetFields(MethodUtilities.BindingFlags);
    }
}
