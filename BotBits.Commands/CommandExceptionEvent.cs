namespace BotBits.Commands
{
    public sealed class CommandExceptionEvent : Event<CommandExceptionEvent>
    {
        public CommandExceptionEvent(IInvokeSource source, ParsedRequest request, CommandException exception)
        {
            this.Exception = exception;
            this.Source = source;
            this.Request = request;
            this.Handled = exception.Ignored;
        }

        public CommandException Exception { get; private set; }
        public IInvokeSource Source { get; private set; }
        public ParsedRequest Request { get; private set; }
        public bool Handled { get; set; }
    }
}