namespace Compendium.Commands
{
    public class CommandContext
    {
        public Player Sender { get; }

        public CommandInfo Command { get; }
        public CommandSource Source { get; }

        public string CommandLine { get; }

        public CommandContext(Player sender, CommandInfo command, CommandSource source, string commandLine)
        {
            Sender = sender;
            Command = command;
            Source = source;
            CommandLine = commandLine;
        }
    }
}