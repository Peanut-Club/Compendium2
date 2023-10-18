using Compendium.Utilities.Reflection;

using MonoMod.Utils;

using System;
using System.Linq;
using System.Reflection;

namespace Compendium.Utilities.Instances
{
    public class InstanceCreator
    {
        public FastReflectionHelper.FastInvoker Handler { get; }

        public Type[] Types { get; }

        public InstanceCreator(ConstructorInfo constructor)
        {
            Handler = constructor.GetFastInvoker();
            Types = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
        }

        public object Invoke(params object[] args)
            => Handler.SafeCall(null, args);
    }
}
