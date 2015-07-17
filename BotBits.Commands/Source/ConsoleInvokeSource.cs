using System.Diagnostics;

namespace BotBits.Commands
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