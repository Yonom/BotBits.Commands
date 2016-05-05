using System;

namespace BotBits.Commands
{
    public sealed class CommandsExtension : Extension<CommandsExtension>
    {
        private class Settings
        {
            public char[] CommandPrefixes { get; }
            public ListeningBehavior ListeningBehavior { get; }

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

        public static bool LoadInto(BotBitsClient client, params char[] commandPrefixes)
        {
            if (commandPrefixes.Length == 0)
                throw new ArgumentException("At least one command prefix must be provided.", nameof(commandPrefixes));
            
            return LoadInto(client, new Settings(commandPrefixes));
        }

        public static bool LoadInto(BotBitsClient client, ListeningBehavior listeningBehavior, params char[] commandPrefixes)
        {
            if (commandPrefixes.Length == 0)
                throw new ArgumentException("At least one command prefix must be provided.", nameof(commandPrefixes));

            return LoadInto(client, new Settings(commandPrefixes, listeningBehavior));
        }
    }
}
