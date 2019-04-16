using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Abstractions
{
    interface IExecutionContext
    {
        IFrame Frame { get; }
        Task<JToken> EvaluateExpressionAsync(string script);
        Task<T> EvaluateExpressionAsync<T>(string script);
        Task<JToken> EvaluateFunctionAsync(string script, params object[] args);
        Task<T> EvaluateFunctionAsync<T>(string script, params object[] args);
        Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle);
    }
}