using System;
using JetBrains.Annotations;

namespace BotBits.Commands
{
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
    public sealed class CommandAttribute : Attribute
    {
        private readonly int _minArgs;
        private readonly string[] _names;

        public string[] Names
        {
            get { return this._names; }
        }

        public int MinArgs
        {
            get { return _minArgs; }
        }

        public string[] Usages { get; set; }

        public string Usage
        {
            get { throw new NotSupportedException(); }
            set { this.Usages = new[] {value}; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute" /> class.
        /// </summary>
        /// <param name="minArgs">The minimum required arguments.</param>
        /// <param name="names">The command name/names.</param>
        public CommandAttribute(int minArgs, params string[] names)
        {
            this._minArgs = minArgs;
            this._names = names;
        }
    }
}
