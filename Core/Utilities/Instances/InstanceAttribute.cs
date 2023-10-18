using System;

namespace Compendium.Utilities.Instances
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InstanceAttribute : Attribute { }
}