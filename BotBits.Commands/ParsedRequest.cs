using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace BotBits.Commands
{
    [DebuggerDisplay("Value = {Value}")]
    public class ParsedRequest
    {
        public ParsedRequest([NotNull] string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            this.Value = value;
            var parts = value.Split(' ');
            this.Type = parts[0];
            this.Args = parts.Skip(1).ToArray();
        }

        public string Value { get; }
        public string Type { get; }
        public string[] Args { get; }
        public int Count => this.Args.Length;

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
            return string.Join(" ", this.Args.Skip(index));
        }
    }
}