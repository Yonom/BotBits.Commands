using System;

namespace BotBits.Commands
{
    /// <summary>
    ///     Represents a general exception that happened while excecuting a command
    /// </summary>
    public class CommandException : Exception
    {
        public bool Ignored { get; private set; }

        public CommandException(bool ignored = false)
        {
            this.Ignored = ignored;
        }

        public CommandException(string message, bool ignored = false)
            : base(message)
        {
            this.Ignored = ignored;
        }

        public CommandException(string message, Exception innerException, bool ignored = false)
            : base(message, innerException)
        {
            this.Ignored = ignored;
        }
    }
}