using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBits.Commands
{
    public sealed class CommandsExtension : Extension<CommandsExtension>
    {
        private class Settings
        {
            public char[] CommandPrefixes { get; set; }
            public ListeningBehavior ListeningBehavior { get; set; }

            public Settings(char[] commandPrefixes, ListeningBehavior listeningBehavior = ListeningBehavior.Both)
            {
                this.CommandPrefixes = commandPrefixes;
                this.ListeningBehavior = listeningBehavior;
            }
        }

        protected override void Initialize(BotBitsClient client, object args)
        {
            var settings = (Settings)args;
            CommandManager.Of(client).CommandPrefixes = settings.CommandPrefixes;
            CommandManager.Of(client).ListeningBehavior = settings.ListeningBehavior;
        }

        public static void LoadInto(BotBitsClient client, params char[] commandPrefixes)
        {
            if (commandPrefixes.Length == 0)
                throw new ArgumentException("At least one command prefix must be provided.", "commandPrefixes");
            
            LoadInto(client, new Settings(commandPrefixes));
        }

        public static void LoadInto(BotBitsClient client, ListeningBehavior listeningBehavior, params char[] commandPrefixes)
        {
            if (commandPrefixes.Length == 0)
                throw new ArgumentException("At least one command prefix must be provided.", "commandPrefixes");

            LoadInto(client, new Settings(commandPrefixes, listeningBehavior));
        }
    }
}
