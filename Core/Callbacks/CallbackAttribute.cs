using Compendium.Enums;
using Compendium.Utilities;

using System;

namespace Compendium.Callbacks
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CallbackAttribute : Attribute
    {
        public Delay Delay { get; set; }
        public Priority Priority { get; set; } = Priority.Normal;
    }
}