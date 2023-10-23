using System;
using System.Threading.Tasks;

namespace Compendium.Execution
{
    public struct TaskInfo
    {
        public Task Task;

        public Action SuccessCallback;
        public Action<Exception> ErrorCallback;
    }
}