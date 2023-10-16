using Compendium.Utilities;

using System;
using System.Collections.Generic;

namespace Compendium.Logging.Ratelimiting
{
    public class LogRateLimit : Disposable
    {
        private object _lock;

        public int Window { get; set; }
        public int Count { get; set; }
        public int Cooldown { get; set; }

        public List<string> RateLimitedSources { get; private set; } = new List<string>();
        public Dictionary<string, LogRateLimitValue> RateLimits { get; private set; } = new Dictionary<string, LogRateLimitValue>();

        public event Action<string, DateTime, DateTime, int> OnRateLimited;
        public event Action<string> OnRateLimitRemoved;

        public LogRateLimit(int logWindow, int logAmount, int logCooldown)
        {
            Window = logWindow;
            Count = logAmount;
            Cooldown = logCooldown;
        }

        public bool Verify(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return true;

            lock (_lock)
            {
                if (!RateLimits.ContainsKey(source))
                    RateLimits.Add(source, new LogRateLimitValue());

                RateLimits[source].Sent++;

                if (RateLimits[source].WindowReset.HasValue && DateTime.Now >= RateLimits[source].WindowReset.Value)
                {
                    RateLimits[source].Sent = 0;
                    RateLimits[source].WindowStart = DateTime.Now;

                    RateLimitedSources.Remove(source);

                    OnRateLimitRemoved?.Invoke(source);
                }
                else
                {
                    if (RateLimits[source].WindowStart.HasValue && RateLimits[source].Sent >= Count)
                    {
                        if ((DateTime.Now - RateLimits[source].WindowStart.Value).TotalMilliseconds >= Window)
                        {
                            RateLimitedSources.Add(source);
                            RateLimits[source].WindowReset = DateTime.Now + TimeSpan.FromMilliseconds(Cooldown);

                            OnRateLimited?.Invoke(source, RateLimits[source].WindowStart.Value, RateLimits[source].WindowReset.Value, RateLimits[source].Sent);
                        }
                    }
                    else
                    {
                        RateLimits[source].WindowStart = DateTime.Now;
                    }
                }

                return !RateLimitedSources.Contains(source);
            }
        }

        public override void DisposeInternal()
        {
            RateLimitedSources.Clear();
            RateLimitedSources = null;

            RateLimits.Clear();
            RateLimits = null;

            base.DisposeInternal();
        }
    }
}