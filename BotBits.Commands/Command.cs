using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BotBits.Commands
{
    public sealed class Command
    {
        public Command(int minArgs, string name, Action<IInvokeSource, ParsedRequest> callback)
            : this(minArgs, new[] { name }, new string[0], callback)
        {
        }

        public Command(int minArgs, string name, string usage, Action<IInvokeSource, ParsedRequest> callback)
            : this(minArgs, new[] { name }, new[] { usage }, callback)
        {
        }

        public Command(int minArgs, string name, string[] usages, Action<IInvokeSource, ParsedRequest> callback)
            : this(minArgs, new[] { name }, usages, callback)
        {
        }

        public Command(int minArgs, string[] names, Action<IInvokeSource, ParsedRequest> callback)
            : this(minArgs, names, new string[0], callback)
        {
        }

        public Command(int minArgs, string[] names, string usage, Action<IInvokeSource, ParsedRequest> callback)
            : this(minArgs, names, new[] { usage }, callback)
        {
        }

        public Command(int minArgs, string[] names, string[] usages, Action<IInvokeSource, ParsedRequest> callback)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (usages == null) throw new ArgumentNullException(nameof(usages));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            this.Names = names;
            this.Usages = usages;
            this.MinArgs = minArgs;
            this.Callback = callback;
        }

        internal Command(BotBitsClient client, Func<IInvokeSource, ParsedRequest, Task> callback)
            : this(client, ExceptionHelper.WrapTryCatch(client, callback), callback.Method)
        {
        }

        internal Command(BotBitsClient client, Action<IInvokeSource, ParsedRequest> callback)
            : this(client, ExceptionHelper.WrapTryCatch(client, callback), callback.Method)
        {
        }

        private Command(BotBitsClient client, Action<IInvokeSource, ParsedRequest> callback, MethodInfo innerMethod)
        {
            var command = (CommandAttribute)innerMethod.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault();
            if (command == null) throw new ArgumentException("The given callback is not a command", nameof(callback));


            this.Names = command.Names ?? new string[0];
            this.Usages = command.Usages ?? new string[0];
            this.MinArgs = command.MinArgs;

            this.Callback = command.DoTransformations(client, this, callback);
        }

        public string[] Names { get; }

        public string[] Usages { get; }

        public int MinArgs { get; }

        public Action<IInvokeSource, ParsedRequest> Callback { get; }
    }
}