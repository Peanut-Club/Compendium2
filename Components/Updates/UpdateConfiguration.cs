using Compendium.Time;

using MEC;

namespace Compendium.Components.Updates
{
    public class UpdateConfiguration
    {
        public Segment Segment { get; set; } = Segment.Update;

        public Delay Delay { get; set; } = new Delay() { IsValid = false, Value = -1 };

        public int Repeats { get; set; } = -1;

        public bool RecordMethodTime { get; set; }
    }
}