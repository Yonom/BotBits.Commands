using System.Diagnostics;

namespace BotBits.Commands.Source
{
    [DebuggerDisplay("Name = {Name}")]
    public class ConsoleInvokeSource : InvokeSourceBase
    {
        public ConsoleInvokeSource(ReplyCallback onReply)
            : base("console", onReply)
        {
        }
    }
}