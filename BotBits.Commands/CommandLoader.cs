using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            if (parameters[1].ParameterType != typeof(ParsedRequest))
                throw GetCommandEx(eventHandler, "Second argument must be of type ParsedCommand.");

            if (eventHandler.ReturnType == typeof (Task))
            {
                var handler = (Func<IInvokeSource, ParsedRequest, Task>)
                    Delegate.CreateDelegate(typeof(Func<IInvokeSource, ParsedRequest, Task>), baseObj, eventHandler);
               return () => CommandManager.Of(this.BotBits).Add(handler);
            }
            if (eventHandler.ReturnType == typeof (void))
            {
                var handler = (Action<IInvokeSource, ParsedRequest>)
                    Delegate.CreateDelegate(typeof (Action<IInvokeSource, ParsedRequest>), baseObj, eventHandler);
                return () => CommandManager.Of(this.BotBits).Add(handler);
            }
            throw GetCommandEx(eventHandler, "Return type must be void (or async Task).");
        }

        protected override Action GetUnbinder(object baseObj, MethodInfo eventHandler)
        {
            var attr = (CommandAttribute)eventHandler.GetCustomAttributes(typeof(CommandAttribute), false).First();
            var cmdname = attr.Names.First();
            Command command;
            CommandManager.Of(this.BotBits).TryGetCommand(cmdname, out command);
            return () => CommandManager.Of(this.BotBits).Remove(command);
        }

        private static Exception GetCommandEx(MethodInfo handler, string reason)
        {
            return new TypeLoadException(
                $"Unable to assign the method {handler.DeclaringType?.FullName}.{handler.Name} to a command handler. {reason}");
        }
    }
}
