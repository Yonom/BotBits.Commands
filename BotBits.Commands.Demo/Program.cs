using System;
using System.Threading;
using BotBits.Commands.Source;

namespace BotBits.Commands.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotBitsClient();

            // Events
            EventLoader.Of(bot).LoadStatic<Program>();

            // Commands
            CommandsExtension.LoadInto(bot, '!', '.');
            CommandLoader.Of(bot).LoadStatic<Program>();

            // Login
            ConnectionManager.Of(bot)
                .EmailLogin("email", "pass")
                .CreateJoinRoom("world");

            // Console commands
            while (true) 
                CommandManager.Of(bot).ReadNextConsoleCommand();
        }

        [Command(0, "hi")]
        static void HiCommand(IInvokeSource source, ParsedCommand command)
        {
            source.Reply("Hi");
        }

        [EventListener]
        static void OnCommand(CommandEvent e)
        {
            Console.WriteLine("Command run: " + e.Command.Value);
        }
    }
}
