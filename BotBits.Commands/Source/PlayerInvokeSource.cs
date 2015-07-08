using System.Diagnostics;

namespace BotBits.Commands.Source
{
    [DebuggerDisplay("Player = {Player}")]
    public class PlayerInvokeSource : InvokeSourceBase
    {
        public PlayerInvokeSource(Player player, ReplyCallback onReply)
            : base(onReply)
        {
            this.Player = player;
        }

        public Player Player { get; private set; }
    }
}