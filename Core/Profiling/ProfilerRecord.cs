using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Compendium.Profiling
{
    public class ProfilerRecord
    {
        private List<ProfilerFrame> _internalFrames;
        private List<ProfilerFrame> _executingFrames;

        private int _framesSinceTicks;
        private int _framesSinceTime;
        private int _framesSinceMemory;

        private int _maxFrames = -1;

        private bool _fullPause;

        private long _frameNumber = 0;

        public int MemoryCaptureFrameCount { get; set; } = -1;
        public int TicksCaptureFrameCount { get; set; } = -1;
        public int TimeCaptureFrameCount { get; set; } = -1;

        public ProfilerMode Mode { get; set; }

        public MethodBase Target { get; }

        public bool IsPaused { get; set; }

        public bool IsActive
        {
            get => Profiler.IsActive(this);
            set
            {
                if (value == IsActive)
                    return;

                if (value)
                    Profiler.Start(Target);
                else
                    Profiler.Stop(Target);
            }
        }

        public ProfilerRecord(MethodBase target, int maxFrames = -1, ProfilerMode mode = ProfilerMode.Patched)
        {
            Target = target;
            Mode = mode;

            _maxFrames = maxFrames;

            if (_maxFrames > 0)
                _internalFrames = new List<ProfilerFrame>(_maxFrames);
            else
                _internalFrames = new List<ProfilerFrame>();
        }

        public bool TryNewFrame(out ProfilerFrame frame)
        {
            if (IsPaused)
            {
                frame = default;
                return false;
            }

            if (_maxFrames != -1 && (_internalFrames.Count >= _maxFrames || _internalFrames.Count >= _internalFrames.Capacity))
            {
                _fullPause = true;
                IsPaused = true;

                frame = default;
                return false;
            }

            frame = new ProfilerFrame();
            frame.IsValid = true;
            frame.Stamp = new ProfilerStamp { Start = DateTime.Now };
            frame.Number = _frameNumber++;

            if (MemoryCaptureFrameCount != -1 && MemoryCaptureFrameCount >= _framesSinceMemory)
                frame.Memory = new ProfilerStat<long> { Start = GC.GetTotalMemory(false), IsRecorded = true };

            if (TicksCaptureFrameCount != -1 && TicksCaptureFrameCount >= _framesSinceTicks)
                frame.Ticks = new ProfilerStat<double> { Start = 1f / Time.smoothDeltaTime, IsRecorded = true };

            if (TimeCaptureFrameCount != -1 && TimeCaptureFrameCount >= _framesSinceTime)
                frame.Time = new ProfilerStat<double> { Start = Time.deltaTime, IsRecorded = true };

            _executingFrames.Add(frame);

            return true;
        }

        public void EndFrame(long number, Exception exception)
        {
            for (int i = 0; i < _executingFrames.Count; i++)
            {
                if (_executingFrames[i].Number == number)
                    EndFrame(_executingFrames[i], exception);
            }

            _executingFrames.RemoveAll(f => f.Number == number);
        }

        public void EndFrame(ProfilerFrame frame, Exception exception)
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

        public void Clear()
        {
            _framesSinceMemory = 0;
            _framesSinceTicks = 0;
            _framesSinceTime = 0;

            _internalFrames.Clear();
            _executingFrames.Clear();

            if (_maxFrames != -1 && IsPaused && _fullPause)
            {
                IsPaused = false;
                _fullPause = false;
            }
        }

        public ProfilerFrame GetPrevious()
            => _internalFrames.ElementAtOrDefault(_internalFrames.Count - 2);

        public ProfilerFrame GetWithHighestMemory()
            => _internalFrames.OrderByDescending(f => f.Memory.Difference).FirstOrDefault();

        public ProfilerFrame GetWithLowestMemory()
            => _internalFrames.OrderBy(f => f.Memory.Difference).FirstOrDefault();

        public ProfilerFrame GetWithHighestTicks()
            => _internalFrames.OrderByDescending(f => f.Ticks.End).FirstOrDefault();

        public ProfilerFrame GetWithLowestTicks()
            => _internalFrames.OrderBy(f => f.Ticks.End).FirstOrDefault();

        public ProfilerFrame GetWithLongestFrameTime()
            => _internalFrames.OrderByDescending(f => f.Time.End).FirstOrDefault();

        public ProfilerFrame GetWithShortestFrameTime()
            => _internalFrames.OrderBy(f => f.Time.End).FirstOrDefault();

        public ProfilerFrame GetLongest()
            => _internalFrames.OrderByDescending(f => f.Execution).FirstOrDefault();

        public ProfilerFrame GetShortest()
            => _internalFrames.OrderBy(f => f.Execution).FirstOrDefault();

        public double GetAverage()
            => _internalFrames.Average(f => f.Execution);

        public double GetAverageMemory()
            => _internalFrames.Average(f => f.Memory.End);

        public double GetAverageTicks()
            => _internalFrames.Average(f => f.Ticks.End);

        public double GetAverageTime()
            => _internalFrames.Average(f => f.Ticks.End);

        public ProfilerFrame[] GetAllFrames()
            => _internalFrames.ToArray();

        public ProfilerFrame[] GetFaultyFrames()
            => _internalFrames.Where(f => f.Exception != null).ToArray();

        public ProfilerFrame[] GetFaultyFramesOfType<TException>() where TException : Exception
            => _internalFrames.Where(f => f.Exception != null && f.Exception is TException).ToArray();

        public ProfilerFrame[] GetExecuting()
            => _executingFrames.ToArray();
    }
}