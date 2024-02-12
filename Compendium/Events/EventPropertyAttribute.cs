using System;

namespace Compendium.Events
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EventPropertyAttribute : Attribute { }
}