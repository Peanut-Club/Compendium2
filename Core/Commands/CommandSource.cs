using System.Collections.Generic;

namespace Compendium.Commands
{
    public class CommandSource
    {
        public static readonly CommandSource RemoteAdmin = new CommandSource();
        public static readonly CommandSource Console = new CommandSource();
        public static readonly CommandSource Player = new CommandSource();

        private readonly List<CommandInfo> _commands = new List<CommandInfo>();
    }
}