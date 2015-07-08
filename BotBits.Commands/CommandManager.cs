using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BotBits.Commands.Source;
using BotBits.Events;

namespace BotBits.Commands
{
    /// <summary>
    ///     Class CommandManager.
    /// </summary>
    public sealed class CommandManager : EventListenerPackage<CommandManager>, IEnumerable<Command>
    {
        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();
        private readonly object _lockObj = new object();

        public char[] CommandPrefixes { get; internal set; }

        public void ReadNextConsoleCommand()
        {
            var text = Console.ReadLine();
            if (text == null) return;

            var cmd = new ParsedCommand(text);
            var source = new ExternalInvokeSource("Console", Console.WriteLine);
            new CommandEvent(source, cmd)
                .RaiseIn(this.BotBits);
        }

        [EventListener(EventPriority.High)]
        private void OnChat(ChatEvent e)
        {
            if (CommandPrefixes.Contains(e.Text[0]))
            {
                var cmd = new ParsedCommand(e.Text.Substring(1));
                var source = new PlayerInvokeSource(e.Player,
                    msg => Chat.Of(this.BotBits).Say(msg));
                new CommandEvent(source, cmd)
                    .RaiseIn(this.BotBits);
            }
        }
        
        [EventListener(EventPriority.High)]
        private void OnCommand(CommandEvent e)
        {
            Command cmd;
            if (this.TryGetCommand(e.Command.Type, out cmd))
            {
                if (e.Handled)
                {
                    return;
                }

                e.Handled = true;
                this.ExecuteCommand(cmd, e.Source, e.Command);
            }
        }

        internal void ExecuteCommand(Command command, IInvokeSource source, ParsedCommand message)
        {
            try
            {
                if (message.Count < command.MinArgs)
                    throw new SyntaxCommandException("Too few arguments. Correct usage: " + 
                        this.GetUsageStr(command, message.Type));

                command.Callback(source, message);
            }
            catch (CommandException ex)
            {
                source.Reply(ex.Message);
            }
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
        public void Add(Action<IInvokeSource, ParsedCommand> callback)
        {
            this.Add(new Command(callback));
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
                if (command.Names.Any(this.ContainsInternal))
                {
                    throw new ArgumentException("A command with the given name has already been registered.");
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