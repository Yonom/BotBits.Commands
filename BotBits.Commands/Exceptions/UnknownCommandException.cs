using System;

namespace BotBits.Commands
{
    public class UnknownCommandException : CommandException
    {
        public UnknownCommandException() : base(true)
        {
        }

        public UnknownCommandException(string message)
            : base(message, true)
        {
        }

        public UnknownCommandException(string message, Exception innerException)
            : base(message, innerException, true)
        {
        }
    }
}
