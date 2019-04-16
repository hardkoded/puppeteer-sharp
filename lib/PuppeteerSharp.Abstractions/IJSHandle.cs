using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IJSHandle
    {
        IExecutionContext ExecutionContext { get; }
        bool Disposed { get; set; }
        Task<IJSHandle> GetPropertyAsync(string propertyName);
        Task<Dictionary<string, IJSHandle>> GetPropertiesAsync();
        Task<object> JsonValueAsync();
        Task<T> JsonValueAsync<T>();
        Task DisposeAsync();
        string ToString();
    }
}