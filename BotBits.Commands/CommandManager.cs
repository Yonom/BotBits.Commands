using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotBits.Events;

namespace BotBits.Commands
{
    /// <summary>
    ///     Class CommandManager.
    /// </summary>
    public sealed class CommandManager : EventListenerPackage<CommandManager>, IEnumerable<Command>
    {
        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lockObj = new object();

        public char[] CommandPrefixes { get; internal set; }
        public ListeningBehavior ListeningBehavior { get; internal set; }

        [Obsolete("Invalid to use \"new\" on this class. Use the static .Of(botBits) method instead.", true)]
        public CommandManager()
        {
        }

        public Thread CreateConsoleCommandReaderThread()
        {
            return this.CreateConsoleCommandReaderThread(new CancellationToken());
        }

        public Thread CreateConsoleCommandReaderThread(CancellationToken cancellationToken)
        {
            return new Thread(() =>
            {
                while (!cancellationToken.IsCancellationRequested) 
                    this.ReadNextConsoleCommand();
            }) {IsBackground = true, Name = "BotBits.Commands.CommandReader"};
        }

        public Command this[string name]
        {
            get
            {
                Command cmd;
                if (!this.TryGetCommand(name, out cmd))
                    throw new InvalidOperationException("Unknown command.");
                return cmd;
            }
        }

        public void ReadNextConsoleCommand()
        {
            var text = Console.ReadLine();
            if (text == null) return;

            var request = new ParsedRequest(text);
            var source = new ConsoleInvokeSource(Console.WriteLine);
            new CommandEvent(source, request)
                .RaiseIn(this.BotBits);
        }

        [EventListener]
        private void OnChat(ChatEvent e)
        {
            if (!this.ListeningBehavior.HasFlag(ListeningBehavior.Chat)) return;
            if (e.Player == Players.Of(this.BotBits).OwnPlayer) return;
            if (e.Text.Length == 0) return;

            if (this.CommandPrefixes.Contains(e.Text[0]))
            {
                var request = new ParsedRequest(e.Text.Substring(1));
                var source = new PlayerInvokeSource(e.Player, PlayerInvokeOrigin.Chat, 
                    msg => Chat.Of(this.BotBits).Say(msg));

                new CommandEvent(source, request)
                    .RaiseIn(this.BotBits);
            }
        }

        [EventListener]
        private void OnPrivateMessage(PrivateMessageEvent e)
        {
            if (!this.ListeningBehavior.HasFlag(ListeningBehavior.PrivateMessage)) return;
            if (e.Message.Length == 0) return;

            if (this.CommandPrefixes.Contains(e.Message[0]))
            {
                var player = Players.Of(this.BotBits).FromUsername(e.Username).FirstOrDefault();
                if (player == null) return;

                var cmd = new ParsedRequest(e.Message.Substring(1));
                var source = new PlayerInvokeSource(player, PlayerInvokeOrigin.PrivateMessage, 
                    msg => Chat.Of(this.BotBits).PrivateMessage(player, msg));

                new CommandEvent(source, cmd)
                    .RaiseIn(this.BotBits);
            }
        }
        
        [EventListener(GlobalPriority.AfterMost)]
        private void OnCommand(CommandEvent e)
        {
            ExceptionHelper.WrapTryCatch(this.BotBits, (source, req) =>
            {
                Command cmd;
                if (this.TryGetCommand(req.Type, out cmd))
                {
                    if (e.Handled)
                    {
                        return;
                    }

                    e.Handled = true;
                    if (req.Count < cmd.MinArgs)
                        throw new SyntaxCommandException(
                            "Too few arguments. Correct usage: " +
                            this.GetUsageStr(cmd, req.Type));

                    cmd.Callback(source, req);
                }
                else
                {
                    throw new UnknownCommandException("Unknown command.");
                }
            })(e.Source, e.Request);
        }

        [EventListener(GlobalPriority.AfterMost)]
        private void OnCommandException(CommandExceptionEvent e)
        {
            if (e.Handled) return;
            if (String.IsNullOrEmpty(e.Exception.Message)) return;

            e.Source.Reply(e.Exception.Message);
        }

        private string GetUsageStr(Command command, string label)
        {
            string[] correctUsages =
                command.Usages.Select(usage => this.CommandPrefixes.First() + label + " " + usage).ToArray();
            return correctUsages.Length > 0
                ? String.Join(" / ", correctUsages)
                : "<unavailable>";
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Command> GetEnumerator()
        {
            lock (this._lockObj)
            {
                return this._commands.Values.ToArray().AsEnumerable().GetEnumerator();
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     Registers the specified command.
        /// </summary>
        /// <param name="callback">The command.</param>
        /// <exception cref="System.ArgumentException">A command with the given name has already been registered.</exception> 
        public void Add(Func<IInvokeSource, ParsedRequest, Task> callback)
        {
            this.Add(new Command(this.BotBits, callback));
        }

        /// <summary>
        ///     Registers the specified command.
        /// </summary>
        /// <param name="callback">The command.</param>
        /// <exception cref="System.ArgumentException">A command with the given name has already been registered.</exception> 
        public void Add(Action<IInvokeSource, ParsedRequest> callback)
        {
            this.Add(new Command(this.BotBits, callback));
        }

        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.ArgumentException">A command with the given name has already been registered.</exception>
        public void Add(Command command)
        {
            lock (this._lockObj)
            {
                var dupes = command.Names.Where(this.ContainsInternal).ToArray();
                if (dupes.Any())
                {
                    throw new ArgumentException($"A command with the given name(s) has already been registered: {String.Join(",", dupes)}");
                }

                foreach (var label in command.Names)
                {
                    this._commands.Add(label, command);
                }
            }
        }

        /// <summary>
        ///     Determines whether the specified command is registered in this <see cref="CommandManager" /> or not.
        /// </summary>
        /// <returns></returns>
        public bool Contains(string name)
        {
            lock (this._lockObj)
            {
                return this.ContainsInternal(name);
            }
        }

        private bool ContainsInternal(string name)
        {
            return this._commands.ContainsKey(name);
        }

        /// <summary>
        ///     Removes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public bool Remove(Command command)
        {
            lock (this._lockObj)
            {
                return this._commands
                    .Where(kv => kv.Value == command)
                    .Select(kv => kv.Key)
                    .ToArray()
                    .All(this._commands.Remove);
            }
        }

        /// <summary>
        /// Tries to get the command object linked with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="command">The command.</param>
        public bool TryGetCommand(string name, out Command command)
        {
            lock (this._lockObj)
            {
                return this._commands.TryGetValue(name, out command);
            }
        }
    }
}