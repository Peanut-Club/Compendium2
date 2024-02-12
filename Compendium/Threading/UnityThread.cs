using Common.Extensions;
using Common.Logging;
using Common.Utilities.Threading;

using MEC;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using UnityEngine;

namespace Compendium.Threading
{
    public class UnityThread : IThreadManager
    {
        public static readonly UnityThread Instance = new UnityThread();

        private readonly ConcurrentQueue<Tuple<ThreadAction, Action>> threadActions;
        private readonly LogOutput log;
        private readonly CoroutineHandle coroutine;

        public int Size => threadActions.Count;
        public int MaxCount { get; set; } 

        public bool IsRunning => Application.isPlaying;

        public UnityThread()
        {
            log = new LogOutput("Unity Thread Manager").Setup();

            threadActions = new ConcurrentQueue<Tuple<ThreadAction, Action>>();

            MaxCount = 10;

            coroutine = Timing.RunCoroutine(OnUpdate(), Segment.FixedUpdate, "Unity Thread Manager");
        }

        ~UnityThread()
        {
            Timing.KillCoroutines(coroutine);

            threadActions.Clear();

            log.Dispose();
        }

        public void Run(ThreadAction threadAction, Action callback)
        {
            if (threadAction is null)
                throw new ArgumentNullException(nameof(threadAction));

            if (threadAction.TargetMethod is null)
            {
                log.Error($"Attempted to enqueue an action without a defined method.");
                return;
            }

            if (!threadAction.TargetMethod.IsStatic && threadAction.TargetObject is null)
            {
                log.Error($"Attempted to enqueue a non-static action without a defined class instance. ({threadAction.TargetMethod.ToName()})");
                return;
            }

            try
            {
                threadActions.Enqueue(new Tuple<ThreadAction, Action>(threadAction, callback));
            }
            catch (Exception ex)
            {
                log.Error($"Failed to enqueue action '{threadAction.TargetMethod.ToName()}':\n{ex}");
            }
        }

        private void UpdateActions()
        {
            if (!IsRunning || Size < 1)
                return;

            var count = 0; 

            while (threadActions.TryDequeue(out var tuple) && count < MaxCount)
            {
                var action = tuple.Item1;
                var callback = tuple.Item2;

                count++;

                if (action.IsFinished)
                    continue;

                var start = DateTime.Now;

                try
                {
                    var result = action.TargetMethod.CallUnsafe(action.TargetObject, action.TargetArgs);

                    if (action.IsMeasure)
                        action.OnRun(null, result, TimeSpan.FromMilliseconds((DateTime.Now - start).TotalMilliseconds));
                    else
                        action.OnRun(null, result, default);

                    callback.Call(null, log.Error);
                }
                catch (Exception ex)
                {
                    if (action.IsMeasure)
                        action.OnRun(ex, null, TimeSpan.FromMilliseconds((DateTime.Now - start).TotalMilliseconds));
                    else
                        action.OnRun(ex, null, default);

                    callback.Call(null, log.Error);

                    log.Error($"An exception has been caught while running action '{action.TargetMethod.ToName()}':\n{ex}");
                }
            }
        }

        private IEnumerator<float> OnUpdate()
        {
            while (IsRunning)
            {
                yield return Timing.WaitForOneFrame;

                try
                {
                    UpdateActions();
                }
                catch (Exception ex)
                {
                    log.Error($"An exception has been caught while updating actions:\n{ex}");
                }
            }
        }
    }
}