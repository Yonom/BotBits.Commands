using JetBrains.Annotations;

namespace BotBits.Commands
{
    public static class InvokeSourceExtensions
    {
        public static PlayerInvokeSource ToPlayerInvokeSource(this IInvokeSource source,
            string errorMessage = "You must call this command as a player.")
        {
            var playerSource = source as PlayerInvokeSource;
            if (playerSource == null) throw new InvalidInvokeSourceCommandException(errorMessage);

            return playerSource;
        }

        public static ConsoleInvokeSource ToConsoleInvokeSource(this IInvokeSource source,
            string errorMessage = "This command is not available in game.")
        {
            var consoleSource = source as ConsoleInvokeSource;
            if (consoleSource == null) throw new InvalidInvokeSourceCommandException(errorMessage);

            return consoleSource;
        }

        /// <summary>
        ///     Replies the specified chat message to the invoke source.
        /// </summary>
        /// <param name="invokeSource">The invoke source.</param>
        /// <param name="message">The chat message.</param>
        /// <param name="args">The object array that contains zero or more items to format.</param>
        [StringFormatMethod("message")]
        public static void Reply(this IInvokeSource invokeSource, string message, params object[] args)
        {
            // ReSharper disable once RedundantStringFormatCall
            invokeSource.Reply(string.Format(message, args));
        }
    }
}