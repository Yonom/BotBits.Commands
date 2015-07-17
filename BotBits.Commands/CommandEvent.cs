namespace BotBits.Commands
{
    public sealed class CommandEvent : Event<CommandEvent>
    {
        public CommandEvent(IInvokeSource source, ParsedRequest request)
        {
            this.Source = source;
            this.Request = request;
        }

        public IInvokeSource Source { get; set; }
        public ParsedRequest Request { get; set; }
        public bool Handled { get; set; }
    }
}