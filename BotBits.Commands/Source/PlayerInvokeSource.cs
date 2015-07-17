using System.Diagnostics;

namespace BotBits.Commands
{
    [DebuggerDisplay("Player = {Player}")]
    public class PlayerInvokeSource : InvokeSourceBase
    {
        public PlayerInvokeSource(Player player, PlayerInvokeOrigin origin, ReplyCallback onReply)
            : base(player.ChatName, onReply)
        {
            this.Player = player;
            this.Origin = origin;
        }

        public Player Player { get; private set; }
        public PlayerInvokeOrigin Origin { get; private set; }
    }
}