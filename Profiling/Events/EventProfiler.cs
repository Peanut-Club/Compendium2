using Compendium.Events;

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Compendium.Profiling.Events
{
    public static class EventProfiler
    {
        private static readonly List<EventProfilerFrame> _internalFrames = new List<EventProfilerFrame>();

        private static int _framesSinceTicks;
        private static int _framesSinceTime;
        private static int _framesSinceMemory;

        private static long _frameNumber = 0;

        public static int MemoryCaptureFrameCount { get; set; } = -1;
        public static int TicksCaptureFrameCount { get; set; } = -1;
        public static int TimeCaptureFrameCount { get; set; } = -1;

        public static bool IsPaused { get; set; }

        public static bool TryNewFrame(EventData data, out EventProfilerFrame frame)
        {
            if (IsPaused)
            {
                frame = default;
                return false;
            }

            frame = new EventProfilerFrame();
            frame.Event = data;
            frame.IsValid = true;
            frame.Stamp = new ProfilerStamp { Start = DateTime.Now };
            frame.Number = _frameNumber++;

            if (MemoryCaptureFrameCount != -1 && MemoryCaptureFrameCount >= _framesSinceMemory)
                frame.Memory = new ProfilerStat<long> { Start = GC.GetTotalMemory(false), IsRecorded = true };

            if (TicksCaptureFrameCount != -1 && TicksCaptureFrameCount >= _framesSinceTicks)
                frame.Ticks = new ProfilerStat<double> { Start = 1f / Time.smoothDeltaTime, IsRecorded = true };

            if (TimeCaptureFrameCount != -1 && TimeCaptureFrameCount >= _framesSinceTime)
                frame.Time = new ProfilerStat<double> { Start = Time.deltaTime, IsRecorded = true };

            return true;
        }

        public static void EndFrame(EventProfilerFrame frame, Exception exception)
        {
            frame.Stamp.End = DateTime.Now;
            frame.Exception = exception;

            if (frame.Memory.IsRecorded)
            {
                frame.Memory.End = GC.GetTotalMemory(false);
                frame.Memory.Difference = frame.Memory.End - frame.Memory.Start;

                _framesSinceMemory = 0;
            }
            else if (MemoryCaptureFrameCount != -1)
            {
                _framesSinceMemory++;
            }

            if (frame.Ticks.IsRecorded)
            {
                frame.Ticks.End = 1f / Time.smoothDeltaTime;
                frame.Ticks.Difference = frame.Ticks.End - frame.Ticks.Start;

                _framesSinceTicks = 0;
            }
            else if (TicksCaptureFrameCount != -1)
            {
                _framesSinceTicks++;
            }

            if (frame.Time.IsRecorded)
            {
                frame.Time.End = Time.deltaTime;
                frame.Time.Difference = frame.Time.End - frame.Time.Start;

                _framesSinceTime = 0;
            }
            else if (TimeCaptureFrameCount != -1)
            {
                _framesSinceTime++;
            }

            frame.Execution = (frame.Stamp.End - frame.Stamp.Start).Ticks / TimeSpan.TicksPerMillisecond;

            _internalFrames.Add(frame);
        }

        public static EventProfilerFrame GetPrevious(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).ElementAtOrDefault(_internalFrames.Count - 2);

        public static EventProfilerFrame GetWithHighestMemory(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderByDescending(f => f.Memory.Difference).FirstOrDefault();

        public static EventProfilerFrame GetWithLowestMemory(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderBy(f => f.Memory.Difference).FirstOrDefault();

        public static EventProfilerFrame GetWithHighestTicks(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderByDescending(f => f.Ticks.End).FirstOrDefault();

        public static EventProfilerFrame GetWithLowestTicks(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderBy(f => f.Ticks.End).FirstOrDefault();

        public static EventProfilerFrame GetWithLongestFrameTime(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderByDescending(f => f.Time.End).FirstOrDefault();

        public static EventProfilerFrame GetWithShortestFrameTime(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderBy(f => f.Time.End).FirstOrDefault();

        public static EventProfilerFrame GetLongest(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderByDescending(f => f.Execution).FirstOrDefault();

        public static EventProfilerFrame GetShortest(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).OrderBy(f => f.Execution).FirstOrDefault();

        public static double GetAverage(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).Average(f => f.Execution);

        public static double GetAverageMemory(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).Average(f => f.Memory.End);

        public static double GetAverageTicks(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).Average(f => f.Ticks.End);

        public static double GetAverageTime(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).Average(f => f.Ticks.End);

        public static EventProfilerFrame[] GetAllFrames(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).ToArray();

        public static EventProfilerFrame[] GetFaultyFrames(EventData data = null)
            => _internalFrames.Where(f => data is null || f.Event == data).Where(f => f.Exception != null).ToArray();

        public static EventProfilerFrame[] GetFaultyFramesOfType<TException>(EventData data = null) where TException : Exception
            => _internalFrames.Where(f => data is null || f.Event == data).Where(f => f.Exception != null && f.Exception is TException).ToArray();

        public static void Clear()
        {
            _framesSinceMemory = 0;
            _framesSinceTicks = 0;
            _framesSinceTime = 0;

            _internalFrames.Clear();
        }
    }
}