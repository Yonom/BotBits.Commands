﻿using System;

namespace BotBits.Commands
{
    public class InvalidInvokeSourceCommandException : CommandException
    {
        public InvalidInvokeSourceCommandException()
        {
        }

        public InvalidInvokeSourceCommandException(string message)
            : base(message)
        {
        }

        public InvalidInvokeSourceCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}