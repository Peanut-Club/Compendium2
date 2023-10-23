using System;

namespace Compendium.Events
{
    public class EventData
    {
        public System.Reflection.EventInfo Event { get; }
        
        public Delegate Delegate { get; }
        public Type InfoType { get; }

        public EventData(System.Reflection.EventInfo eventInfo, Delegate del, Type infoType)
        {
            Event = eventInfo;
            Delegate = del;
            InfoType = infoType;
        }
    }
}