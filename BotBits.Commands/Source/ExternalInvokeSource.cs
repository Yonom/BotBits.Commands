using System.Diagnostics;

namespace BotBits.Commands.Source
{
    [DebuggerDisplay("Name = {Name}")]
    public class ExternalInvokeSource : InvokeSourceBase
    {
        public string Name { get; private set; }

        public ExternalInvokeSource(string name, ReplyCallback onReply)
            : base(onReply)
        {
            this.Name = name;
        }
    }
}