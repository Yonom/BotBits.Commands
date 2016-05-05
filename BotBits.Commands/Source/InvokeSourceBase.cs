namespace BotBits.Commands
{
    public abstract class InvokeSourceBase : IInvokeSource
    {
        private readonly ReplyCallback _onReply;

        protected InvokeSourceBase(string name, ReplyCallback onReply)
        {
            this.Name = name;
            this._onReply = onReply;
        }

        public string Name { get; }

        public void Reply(string message)
        {
            this._onReply(message);
        }
    }
}