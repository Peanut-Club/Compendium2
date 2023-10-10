using Compendium.Utilities.Reflection;

using System;

namespace Compendium.Components.Updates
{
    public class UpdateInfo
    {
        public Action Target { get; }

        public UpdateConfiguration Configuration { get; }

        public DateTime LastCall { get; set; }
        public DateTime NextCall { get; set; }

        public CallStats<Action> Stats { get; }

        public bool CanCall { get => Target != null && DateTime.Now >= NextCall; }

        public UpdateInfo(Action target, UpdateConfiguration config)
        {
            Target = target;
            Configuration = config;

            Stats = CallStats<Action>.Create(Target);

            LastCall = NextCall = DateTime.Now;
        }

        public void Call()
        {
            if (!CanCall)
                return;

            var start = DateTime.Now;

            Target.SafeCall();

            if (Configuration.RecordMethodTime)
                Stats.Record((DateTime.Now - start).Ticks / TimeSpan.TicksPerMillisecond);

            SetCallTimes();
        }

        public void SetCallTimes()
        {
            LastCall = DateTime.Now;

            if (Configuration.Delay.IsValid && Configuration.Delay.Value > 0)
                NextCall = DateTime.Now + TimeSpan.FromMilliseconds(Configuration.Delay.Value);
            else
                NextCall = DateTime.Now;
        }
    }
}