using System;

namespace Compendium.Instances
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstantiateAttribute : Attribute { }
}