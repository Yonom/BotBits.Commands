namespace BotBits.Commands
{
    public enum PlayerInvokeOrigin
    {
        Chat,
        PrivateMessage
    }

    public static class PlayerInvokeOriginExtensions
    {
        public static void RequireFor(this PlayerInvokeOrigin origin, IInvokeSource source,
            string errorMessage = "Command is not available here.")
        {
            var playerSource = source as PlayerInvokeSource;
            if (playerSource != null)
            {
                if (playerSource.Origin != origin)
                    throw new InvalidInvokeOriginCommandException(errorMessage);
            }
        }
    }
}