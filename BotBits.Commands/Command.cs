using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BotBits.Commands
{
    public sealed class Command
    {
        public Command(string[] names, string[] usages, int minArgs, Action<IInvokeSource, ParsedRequest> callback)
        {
            if (names == null) throw new ArgumentNullException("names");
            if (usages == null) throw new ArgumentNullException("usages");
            if (callback == null) throw new ArgumentNullException("callback");

            this.Names = names;
            this.Usages = usages;
            this.MinArgs = minArgs;
            this.Callback = callback;
        }

        internal Command(Action<IInvokeSource, ParsedRequest> callback)
        {
            this.Callback = callback;

            var method = callback.Method;
            var command = (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault();
            if (command == null) throw new ArgumentException("The given callback is not a command", "callback");

            this.Names = command.Names ?? new string[0];
            this.Usages = command.Usages ?? new string[0];
            this.MinArgs = command.MinArgs;
        }

        public string[] Names { get; private set; }

        public string[] Usages { get; private set; }

        public int MinArgs { get; private set; }

        public Action<IInvokeSource, ParsedRequest> Callback { get; private set; }
    }
}