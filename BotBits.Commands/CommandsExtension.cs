using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBits.Commands
{
    public sealed class CommandsExtension : Extension<CommandsExtension>
    {
        protected override void Initialize(BotBitsClient client, object args)
        {
            var prefixes = (char[])args;
            CommandManager.Of(client).CommandPrefixes = prefixes;
        }

        public static void LoadInto(BotBitsClient client, params char[] commandPrefixes)
        {
            if (commandPrefixes.Length == 0)
                throw new ArgumentException("At least one command prefix must be provided.", "commandPrefixes");

            LoadInto(client, (object)commandPrefixes);
        }
    }
}
