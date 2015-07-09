using System;

namespace BotBits.Commands
{
    public class SyntaxCommandException : CommandException
    {
        public SyntaxCommandException(string message)
            : base(message)
        {
        }
    }
}