using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal static class BindingUtils
    {
        internal static string PageBindingInitString(string type, string name)
            => EvaluationString(
                @"function addPageBinding(type, bindingName) {
                    const win = window;
                    const binding = win[bindingName];

                    win[bindingName] = (...args) => {
                        const me = window[bindingName];
                        let callbacks = me.callbacks;
                        if (!callbacks) {
                            callbacks = new Map();
                            me.callbacks = callbacks;
                        }
                        const seq = (me.lastSeq || 0) + 1;
                        me.lastSeq = seq;
                        const promise = new Promise((resolve, reject) => {
                            return callbacks.set(seq, {resolve, reject});
                        });
                        binding(JSON.stringify({type, name: bindingName, seq, args}));
                        return promise;
                    };
                }",
                type,
                name);

        internal static string EvaluationString(string fun, params object[] args)
        {
            return $"({fun})({string.Join(",", args.Select(SerializeArgument))})";

            string SerializeArgument(object arg)
            {
                return arg == null
                    ? "undefined"
                    : JsonConvert.SerializeObject(arg, JsonHelper.DefaultJsonSerializerSettings);
            }
        }

        internal static async Task<object> ExecuteBindingAsync(BindingCalledResponse e, Dictionary<string, Delegate> pageBindings)
        {
            const string taskResultPropertyName = "Result";
            object result;
            var binding = pageBindings[e.BindingPayload.Name];
            var methodParams = binding.Method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();

            var args = e.BindingPayload.Args.Select((token, i) => token.ToObject(methodParams[i])).ToArray();

            result = binding.DynamicInvoke(args);
            if (result is Task taskResult)
            {
                await taskResult.ConfigureAwait(false);

                if (taskResult.GetType().IsGenericType)
                {
                    // the task is already awaited and therefore the call to property Result will not deadlock
                    result = taskResult.GetType().GetProperty(taskResultPropertyName).GetValue(taskResult);
                }
            }

            return result;
        }
    }
}

