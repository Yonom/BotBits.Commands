namespace BotBits.Commands
{
    public interface IInvokeSource
    {
        string Name { get; }
        void Reply(string message);
    }
}