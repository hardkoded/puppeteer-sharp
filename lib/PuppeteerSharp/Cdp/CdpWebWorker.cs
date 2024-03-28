// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpWebWorker : WebWorker
{
    private readonly ILogger _logger;
    private readonly Func<ConsoleType, IJSHandle[], StackTrace, Task> _consoleAPICalled;
    private readonly Action<EvaluateExceptionResponseDetails> _exceptionThrown;
    private readonly string _id;
    private readonly TargetType _targetType;

    internal CdpWebWorker(
        CDPSession client,
        string url,
        string targetId,
        TargetType targetType,
        Func<ConsoleType, IJSHandle[], StackTrace, Task> consoleAPICalled,
        Action<EvaluateExceptionResponseDetails> exceptionThrown) : base(url)
    {
        _logger = client.Connection.LoggerFactory.CreateLogger<WebWorker>();
        _id = targetId;
        Client = client;
        _targetType = targetType;
        World = new IsolatedWorld(null, this, new TimeoutSettings(), true);
        _consoleAPICalled = consoleAPICalled;
        _exceptionThrown = exceptionThrown;
        client.MessageReceived += OnMessageReceived;

        _ = client.SendAsync("Runtime.enable").ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception!.Message);
                }
            },
            TaskScheduler.Default);

        _ = client.SendAsync("Log.enable").ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception!.Message);
                }
            },
            TaskScheduler.Default);
    }

    /// <inheritdoc/>
    public override CDPSession Client { get; }

    internal override IsolatedWorld World { get; }

    /// <summary>
    /// Closes the worker.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the worker is closed.</returns>
    public override async Task CloseAsync()
    {
        switch (_targetType)
        {
            case TargetType.ServiceWorker:
            case TargetType.SharedWorker:
                // For service and shared workers we need to close the target and detach to allow
                // the worker to stop.
                await Client.Connection.SendAsync(
                    "Target.closeTarget",
                    new TargetCloseTargetRequest()
                    {
                        TargetId = _id,
                    }).ConfigureAwait(false);

                await Client.Connection.SendAsync(
                    "Target.detachFromTarget",
                    new TargetDetachFromTargetRequest()
                    {
                        SessionId = Client.Id,
                    }).ConfigureAwait(false);
                break;
            default:
                await EvaluateFunctionAsync(@"() => {
                        self.close();
                    }").ConfigureAwait(false);
                break;
        }
    }

    private async void OnMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            switch (e.MessageID)
            {
                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(e.MessageData.ToObject<RuntimeExecutionContextCreatedResponse>(true));
                    break;
                case "Runtime.consoleAPICalled":
                    await OnConsoleAPICalledAsync(e).ConfigureAwait(false);
                    break;
                case "Runtime.exceptionThrown":
                    OnExceptionThrown(e.MessageData.ToObject<RuntimeExceptionThrownResponse>(true));
                    break;
            }
        }
        catch (Exception ex)
        {
            var message = $"Worker failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
            _logger.LogError(ex, message);
            Client.Close(message);
        }
    }

    private void OnExceptionThrown(RuntimeExceptionThrownResponse e) => _exceptionThrown(e.ExceptionDetails);

    private async Task OnConsoleAPICalledAsync(MessageEventArgs e)
    {
        var consoleData = e.MessageData.ToObject<PageConsoleResponse>(true);
        await _consoleAPICalled(
            consoleData.Type,
            consoleData.Args.Select(i => new CdpJSHandle(World, i)).ToArray(),
            consoleData.StackTrace)
                .ConfigureAwait(false);
    }

    private void OnExecutionContextCreated(RuntimeExecutionContextCreatedResponse e)
    {
        if (!World.HasContext)
        {
            World.SetNewContext(Client, e.Context, World);
        }
    }
}
