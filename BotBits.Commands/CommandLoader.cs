using System;
using System.Diagnostics;
using System.Reflection;
using BotBits.Commands.Source;

namespace BotBits.Commands
{
    public sealed class CommandLoader : LoaderBase<CommandLoader>
    {
        [Obsolete("Invalid to use \"new\" on this class. Use the static CommandLoader.Of(botBits) method instead.", true)]
        public CommandLoader()
        {
        }

        protected override bool ShouldLoad(MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(CommandAttribute), true);
        }

        protected override Action GetBinder(object baseObj, MethodInfo eventHandler)
        {
            ParameterInfo[] parameters = eventHandler.GetParameters();
            if (parameters.Length != 2)
                throw GetCommandEx(eventHandler, "Too few or too many parameters. Command handlers must have exactly two parameters.");

            if (parameters[0].ParameterType != typeof(IInvokeSource))
                throw GetCommandEx(eventHandler, "First argument must be of type IInvokeSource.");
            if (parameters[1].ParameterType != typeof(ParsedCommand))
                throw GetCommandEx(eventHandler, "Second argument must be of type ParsedCommand.");

            var handler = (Action<IInvokeSource, ParsedCommand>)
                Delegate.CreateDelegate(typeof(Action<IInvokeSource, ParsedCommand>), baseObj, eventHandler);

            return () => CommandManager.Of(this.BotBits).Add(handler);
        }

        private static Exception GetCommandEx(MethodInfo handler, string reason)
        {
            Debug.Assert(handler.DeclaringType != null, "handler.DeclaringType != null");
            return
                new TypeLoadException(String.Format("Unable to assign the method {0}.{1} to a command handler. {2}",
                    handler.DeclaringType.FullName, handler.Name, reason));
        }
    }
}
