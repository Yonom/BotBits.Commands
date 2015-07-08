namespace BotBits.Commands.Source
{
    public abstract class InvokeSourceBase : IInvokeSource
    {
        private readonly ReplyCallback _onReply;

        protected InvokeSourceBase(ReplyCallback onReply)
        {
            this._onReply = onReply;
        }

        public void Reply(string message)
        {
            this._onReply(message);
        }
    }
}