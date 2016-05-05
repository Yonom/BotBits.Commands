using System;
using JetBrains.Annotations;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace BotBits.Commands
{
    [AttributeUsage(AttributeTargets.Method), MeansImplicitUse]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandAttribute" /> class.
        /// </summary>
        /// <param name="minArgs">The minimum required arguments.</param>
        /// <param name="names">The command name/names.</param>
        public CommandAttribute(int minArgs, params string[] names)
        {
            this.MinArgs = minArgs;
            this.Names = names;
        }

        public string[] Names { get; }
        public int MinArgs { get; }
        public string[] Usages { get; set; }

        public string Usage
        {
            get { throw new NotSupportedException(); }
            set { this.Usages = new[] { value }; }
        }

        protected internal virtual Action<IInvokeSource, ParsedRequest> DoTransformations(BotBitsClient client, Command command, Action<IInvokeSource, ParsedRequest> request)
        {
            return request;
        }
    }
}