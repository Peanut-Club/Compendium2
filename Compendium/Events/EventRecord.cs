namespace Compendium.Events
{
    public class EventRecord
    {
        public float Maximum { get; set; } = -1f;
        public float Minimum { get; set; } = -1f;
        public float Average { get; set; } = -1;
        public float Last { get; set; } = -1f;

        public long Executions { get; set; } = 0;
        public long FailedExecutions { get; set; } = 0;
        public long SuccesfullExecutions { get; set; } = 0;

        public bool Recorded { get; set; }

        public void Update(float time, bool isSuccess)
        {
            if (!Recorded || time > Maximum)
                Maximum = time;

            if (!Recorded || time < Minimum)
                Minimum = time;

            Last = time;
            Average = (Maximum + Minimum) / 2f;

            Executions++;

            if (isSuccess)
                SuccesfullExecutions++;
            else
                FailedExecutions++;

            Recorded = true;
        }

        public void Reset()
        {
            Maximum = -1f;
            Minimum = -1f;
            Average = -1f;
            Last = -1f;
            Executions = 0;
            FailedExecutions = 0;
            SuccesfullExecutions = 0;
            Recorded = false;
        }
    }
}