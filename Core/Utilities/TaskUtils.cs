using System.Threading.Tasks;

namespace Compendium.Utilities
{
    public static class TaskUtils
    {
        public static bool IsFinished(this Task task, bool onlySuccess = false)
            => task.IsCompleted || (!onlySuccess && (task.IsFaulted || task.IsCanceled));

        public static bool IsError(this Task task)
            => task.IsFaulted || task.IsCanceled || task.Exception != null;

        public static bool IsFinished<TResult>(this Task<TResult> task, bool onlySuccess = false)
            => task.IsCompleted || (!onlySuccess && (task.IsFaulted || task.IsCanceled));

        public static bool IsError<TResult>(this Task<TResult> task)
            => task.IsFaulted || task.IsCanceled || task.Exception != null;
    }
}