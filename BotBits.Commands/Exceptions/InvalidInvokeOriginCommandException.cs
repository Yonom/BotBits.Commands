using System;

namespace BotBits.Commands
{
    public class InvalidInvokeOriginCommandException : CommandException
    {
        public InvalidInvokeOriginCommandException()
        {
        }

        public InvalidInvokeOriginCommandException(string message)
            : base(message)
        {
        }

        public InvalidInvokeOriginCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}