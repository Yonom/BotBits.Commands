using System;

namespace BotBits.Commands
{
    public class SyntaxCommandException : Exception
    {
        public SyntaxCommandException(string message)
            : base(message)
        {
        }
    }
}