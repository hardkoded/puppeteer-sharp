using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp
{
    internal class Binding
    {
        public Binding(string name, Delegate fn)
        {
            Name = name;
            Function = fn;
        }

        public string Name { get; }

        public Delegate Function { get; }

        internal async Task RunAsync(
            ExecutionContext context,
            int id,
            object[] args,
            bool isTrivial)
        {
            var garbage = new List<Task>();

            try
            {
                if (!isTrivial)
                {
                    // Getting non-trivial arguments.
                    var handles = await context.EvaluateFunctionHandleAsync(
                        @"(name, seq) => {
                            return globalThis[name].args.get(seq);
                        }",
                        Name,
                        id).ConfigureAwait(false);

                    try
                    {
                        var properties = await handles.GetPropertiesAsync().ConfigureAwait(false);

                        foreach (var kv in properties)
                        {
                            var handle = kv.Value;

                            // This is not straight-forward since some arguments can stringify, but
                            // aren't plain objects so add subtypes when the use-case arises.
                            if (int.TryParse(kv.Key, out var index) && args.Length > index)
                            {
                                switch (handle.RemoteObject.Subtype)
                                {
                                    case RemoteObjectSubtype.Node:
                                        args[index] = handle;
                                        break;

                                    default:
                                        garbage.Add(handle.DisposeAsync().AsTask());
                                        break;
                                }
                            }
                            else
                            {
                                garbage.Add(handle.DisposeAsync().AsTask());
                            }
                        }
                    }
                    finally
                    {
                        await handles.DisposeAsync().ConfigureAwait(false);
                    }
                }

                const string taskResultPropertyName = "Result";
                var result = await BindingUtils.ExecuteBindingAsync(Function, args).ConfigureAwait(false);
                if (result is Task taskResult)
                {
                    await taskResult.ConfigureAwait(false);

                    if (taskResult.GetType().IsGenericType)
                    {
                        // the task is already awaited and therefore the call to property Result will not deadlock
                        result = taskResult.GetType().GetProperty(taskResultPropertyName)?.GetValue(taskResult);
                    }
                }

                await context.EvaluateFunctionAsync(
                    @"(name, seq, result) => {
                        const callbacks = globalThis[name].callbacks;
                        callbacks.get(seq).resolve(result);
                        callbacks.delete(seq);
                    }",
                    Name,
                    id,
                    result).ConfigureAwait(false);

                foreach (var arg in args)
                {
                    if (arg is JSHandle handle)
                    {
                        garbage.Add(handle.DisposeAsync().AsTask());
                    }
                }
            }
            catch (Exception ex)
            {
                // Get the exception thrown by the function
                if (ex is TargetInvocationException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                try
                {
                    await context.EvaluateFunctionAsync(
                        @"(name, seq, message, stack) => {
                        const error = new Error(message);
                        error.stack = stack;
                        const callbacks = globalThis[name].callbacks;
                        callbacks.get(seq).reject(error);
                        callbacks.delete(seq);
                    }",
                        Name,
                        id,
                        ex.Message,
                        ex.StackTrace).ConfigureAwait(false);
                }
                catch (Exception cleanupException)
                {
                    var logger = context.Client.Connection.LoggerFactory.CreateLogger<Binding>();
                    logger.LogError(cleanupException.ToString());
                }
            }
            finally
            {
                await Task.WhenAll(garbage.ToArray()).ConfigureAwait(false);
            }
        }
    }
}
