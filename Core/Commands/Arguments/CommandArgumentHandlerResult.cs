namespace Compendium.Commands.Arguments
{
    public struct CommandArgumentHandlerResult
    {
        public readonly bool IsSuccess;
        public readonly int FaultyArgumentPosition;
        public readonly string FaultMessage;

        public CommandArgumentHandlerResult(bool success, int errorIndex, string errorMsg)
        {
            IsSuccess = success;
            FaultyArgumentPosition = errorIndex;
            FaultMessage = errorMsg;
        }
    }
}