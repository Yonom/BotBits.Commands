using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotBits.Commands
{
    public sealed class CommandExceptionEvent : Event<CommandExceptionEvent>
    {
        public CommandException Exception { get; private set; }
        public IInvokeSource Source { get; private set; }
        public ParsedRequest Request { get; private set; }
        public bool Handled { get; set; }

        public CommandExceptionEvent(IInvokeSource source, ParsedRequest request, CommandException exception)
        {
            this.Exception = exception;
            this.Source = source;
            this.Request = request;
            this.Handled = exception.Ignored;
        }
    }
}
