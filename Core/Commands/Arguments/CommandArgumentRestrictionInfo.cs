namespace Compendium.Commands.Arguments
{
    public abstract class CommandArgumentRestrictionInfo
    {
        public abstract bool IsRestricted(CommandContext ctx, object value);
    }
}