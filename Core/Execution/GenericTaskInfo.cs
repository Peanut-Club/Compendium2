using System;
using System.Threading.Tasks;

namespace Compendium.Execution
{
    public struct GenericTaskInfo<TResult>
    {
        public Task<TResult> Task;

        public Action<TResult> SuccessCallback;
        public Action<Exception> ErrorCallback;
    }
}