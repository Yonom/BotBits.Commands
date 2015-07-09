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
            CommandLoader
                .Of(bot)
                .LoadStatic<Program>();

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
            var player = source.ToPlayerInvokeSource().Player;
            source.Reply("Hello world {0}!", player.Username);
        }

        [EventListener]
        static void OnCommand(CommandEvent e)
        {
            Console.WriteLine("Command run: " + e.Command.Type + " by " + e.Source.Name);
        }
    }
}
