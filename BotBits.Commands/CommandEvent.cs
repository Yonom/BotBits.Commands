using BotBits.Commands.Source;

namespace BotBits.Commands
{
    public sealed class CommandEvent : Event<CommandEvent>
    {
        public CommandEvent(IInvokeSource source, ParsedCommand command)
        {
            this.Source = source;
            this.Command = command;
        }

        public IInvokeSource Source { get; set; }
        public ParsedCommand Command { get; set; }
        public bool Handled { get; set; }
    }
}