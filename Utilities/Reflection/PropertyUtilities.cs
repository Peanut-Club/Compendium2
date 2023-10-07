using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compendium.Utilities.Reflection
{
    public static class PropertyUtilities
    {
        public static PropertyInfo[] GetAllProperties(this Type type)
            => type.GetProperties(MethodUtilities.BindingFlags);
    }
}
