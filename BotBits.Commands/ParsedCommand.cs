using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace BotBits.Commands
{
    [DebuggerDisplay("Value = {Value}")]
    public class ParsedCommand
    {
        public ParsedCommand([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.Value = value;
            string[] parts = value.Split(' ');
            this.Type = parts[0];
            this.Args = parts.Skip(1).ToArray();
        }

        public string Value { get; private set; }
        public string Type { get; private set; }
        public string[] Args { get; private set; }

        public int Count
        {
            get { return this.Args.Length; }
        }

        [Pure]
        public int GetInt(int index)
        {
            try
            {
                return Convert.ToInt32(this.Args[index]);
            }
            catch (FormatException)
            {
                throw new SyntaxCommandException("Could not convert parameter " + index + " to integer.");
            }
            catch (OverflowException)
            {
                throw new SyntaxCommandException("Integer at parameter " + index + " was too big.");
            }
        }

        [Pure]
        public string GetTrail(int index)
        {
            return String.Join(" ", this.Args.Skip(index));
        }
    }
}