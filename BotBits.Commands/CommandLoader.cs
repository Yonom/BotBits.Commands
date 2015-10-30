using System;
using System.Diagnostics;
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

        private static Exception GetCommandEx(MethodInfo handler, string reason)
        {
            return
                new TypeLoadException(String.Format("Unable to assign the method {0}.{1} to a command handler. {2}",
                    handler.DeclaringType?.FullName, handler.Name, reason));
        }
    }
}
