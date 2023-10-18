using System;

namespace Compendium.Logging.Ratelimiting
{
    public class LogRateLimitValue
    {
        public int Sent { get; set; } = 0;

        public DateTime? WindowStart { get; set; }
        public DateTime? WindowReset { get; set; }
    }
}