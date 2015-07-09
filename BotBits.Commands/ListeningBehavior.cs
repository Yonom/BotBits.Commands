using System;

namespace BotBits.Commands
{
    [Flags]
    public enum ListeningBehavior
    {
        None,
        Chat = 1 << 0,
        PrivateMessage = 1 << 1,
        Both = Chat | PrivateMessage
    }
}