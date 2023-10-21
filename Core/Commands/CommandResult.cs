using System;
using System.Text;

namespace Compendium.Commands
{
    public struct CommandResult
    {
        public bool IsSuccess;
        public bool IsBlocking;

        public string Response;

        public Exception Exception;

        public CommandResult(bool success, bool blocking, string response, Exception exception)
        {
            IsSuccess = success;
            IsBlocking = blocking;
            Response = response;
            Exception = exception;
        }

        public CommandResult(string response) : this(true, true, response, null) { }
        public CommandResult(params string[] response) : this(true, true, string.Join("\n", response), null) { }
        public CommandResult(StringBuilder stringBuilder) : this(true, true, stringBuilder.ToString(), null) { }

        public CommandResult(Exception exception, string response) : this(false, false, response, exception) { }
        public CommandResult(Exception exception, params object[] response) : this(false, false, string.Join("\n", response), exception) { }
    }
}