using System;
using System.Threading;
using BotBits.Events;

namespace BotBits.Commands.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotBitsClient();

            // Events
            EventLoader
                .Of(bot)
                .LoadStatic<Program>();

            // Commands
            CommandsExtension.LoadInto(bot, '!', '.');

            CommandLoader
                .Of(bot)
                .LoadStatic<Program>();
            CommandLoader
                .Of(bot)
                .LoadStatic<Program>();

            // Login
            Login.Of(bot)
                .AsGuest()
                .CreateJoinRoom("PW01");

            // Console commands
            while (true) 
                CommandManager.Of(bot).ReadNextConsoleCommand();
        }

        [Command(0, "hi")]
        static void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            var player = source.ToPlayerInvokeSource().Player;
            source.Reply("Hello world {0}!", player.Username);
        }

        [EventListener]
        static void OnCommand(CommandEvent e)
        {
            Console.WriteLine("Command run: " + e.Request.Type + " by " + e.Source.Name);
        }
    }
}
