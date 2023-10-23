using System;
using System.Reflection;

namespace Compendium.Events
{
    public struct EventListenerData
    {
        public readonly EventDescriptor Event;
        public readonly MethodBase Method;
        public readonly Delegate Delegate;
    }
}