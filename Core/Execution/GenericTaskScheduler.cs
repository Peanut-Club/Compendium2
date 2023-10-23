using Compendium.Utilities;
using Compendium.Utilities.Reflection;

using MEC;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Compendium.Execution
{
    public static class GenericTaskScheduler<TResult>
    {
        private static readonly Stack<GenericTaskInfo<TResult>> _taskStack = new Stack<GenericTaskInfo<TResult>>();

        public static CoroutineHandle Coroutine { get; private set; }

        static GenericTaskScheduler()
            => Coroutine = Timing.RunCoroutine(ProcessTasks());

        public static void Schedule(Task<TResult> task, Action<TResult> callback = null, Action<Exception> errorCallback = null)
            => _taskStack.Push(new GenericTaskInfo<TResult> { Task = task, SuccessCallback = callback, ErrorCallback = errorCallback });

        private static IEnumerator<float> ProcessTasks()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.15f);

                if (_taskStack.Count > 0)
                {
                    var taskInfo = _taskStack.Pop();
                    var task = taskInfo.Task;

                    if (task.Status != TaskStatus.Running)
                        try { task.Start(); } catch { }

                    while (!task.IsFinished())
                        yield return Timing.WaitForOneFrame;

                    if (task.IsError())
                        taskInfo.ErrorCallback.SafeCall(task.Exception);
                    else
                        taskInfo.SuccessCallback.SafeCall(task.Result);
                }
            }
        }
    }
}