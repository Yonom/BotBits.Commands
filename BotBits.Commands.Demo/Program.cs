using System;

namespace BotBits.Commands.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
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

            // Login
            Login.Of(bot)
                .AsGuest()
                .CreateJoinRoom("PW01");

            // Console commands
            while (true) CommandManager.Of(bot).ReadNextConsoleCommand();
        }

        [Command(0, "hi")]
        private static void HiCommand(IInvokeSource source, ParsedRequest request)
        {
            var player = source.ToPlayerInvokeSource().Player;
            source.Reply("Hello world {0}!", player.Username);
        }

        [EventListener]
        private static void OnCommand(CommandEvent e)
        {
            Console.WriteLine("Command run: " + e.Request.Type + " by " + e.Source.Name);
        }
    }
}