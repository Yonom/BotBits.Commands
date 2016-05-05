﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotBits.Commands
{
    internal static class ExceptionHelper
    {
        public static Action<IInvokeSource, ParsedRequest> WrapTryCatch
            (BotBitsClient client, Action<IInvokeSource, ParsedRequest> callback)
        {
            return (source, req) =>
            {
                try
                {
                    callback(source, req);
                }
                catch (CommandException ex)
                {
                    new CommandExceptionEvent(source, req, ex)
                        .RaiseIn(client);
                }
            };
        }

        public static Action<IInvokeSource, ParsedRequest> WrapTryCatch(
            BotBitsClient client, Func<IInvokeSource, ParsedRequest, Task> callback)
        {
            return (source, req) =>
            {
                callback(source, req).ContinueWith(task =>
                    Scheduler.Of(client).Schedule(() =>
                    {
                        if (task.Exception == null) return;
                        var ex = task.Exception.InnerExceptions.FirstOrDefault() as CommandException;
                        if (ex != null)
                        {
                            new CommandExceptionEvent(source, req, ex)
                                .RaiseIn(client);
                        }
                        else
                        {
                            throw task.Exception.InnerExceptions.FirstOrDefault() ?? task.Exception;
                        }
                    }), TaskContinuationOptions.OnlyOnFaulted);
            };
        }
    }
}