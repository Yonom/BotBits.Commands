namespace BotBits.Commands.Source
{
    public interface IInvokeSource
    {
        string Name { get; }
        void Reply(string message);
    }
}