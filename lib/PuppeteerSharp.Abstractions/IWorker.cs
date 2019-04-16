using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IWorker
    {
        string Url { get; }
        Task<T> EvaluateExpressionAsync<T>(string script);
        Task<IJSHandle> EvaluateExpressionHandleAsync(string script);
    }

}
