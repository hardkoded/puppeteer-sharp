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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpWebWorker : WebWorker
{
    private readonly ILogger _logger;
    private readonly Func<ConsoleType, IJSHandle[], StackTrace, Task> _consoleAPICalled;
    private readonly Action<EvaluateExceptionResponseDetails> _exceptionThrown;
    private readonly Func<bool> _hasOwnerExceptionListeners;
    private readonly Action<RemoteException> _forwardException;
    private readonly string _id;
    private readonly TargetType _targetType;
    private readonly TaskCompletionSource<bool> _workerScriptLoaded = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal CdpWebWorker(
        CDPSession client,
        CdpTarget target,
        Func<ConsoleType, IJSHandle[], StackTrace, Task> consoleAPICalled,
        Action<EvaluateExceptionResponseDetails> exceptionThrown,
        NetworkManager networkManager = null,
        Func<bool> hasOwnerExceptionListeners = null,
        Action<RemoteException> forwardException = null) : base(target.Url)
    {
        _hasOwnerExceptionListeners = hasOwnerExceptionListeners ?? (() => false);
        _forwardException = forwardException ?? (_ => { });
        _logger = client.Connection.LoggerFactory.CreateLogger<WebWorker>();
        _id = target.TargetId;
        Client = client;
        _targetType = target.Type;
        World = new IsolatedWorld(null, this, new TimeoutSettings(), true);
        _consoleAPICalled = consoleAPICalled;
        _exceptionThrown = exceptionThrown;
        client.MessageReceived += OnMessageReceived;

        // Both listeners (the isolated world above and this worker) are now attached, so replay any
        // init events Chrome already delivered on this freshly-attached worker session
        // (Inspector.workerScriptLoaded, Runtime.executionContextCreated) to both of them. Without
        // this, those one-shot events race the subscriptions and can be dropped, leaving
        // EvaluateFunctionAsync awaiting _workerScriptLoaded forever.
        client.FlushEarlyMessages();

        // Service and shared workers create their own NetworkManager.
        var netManager = networkManager ?? CreateOwnNetworkManager(target, client);

        // This might fail if the target is closed before we receive all execution contexts.
        _ = netManager.AddClientAsync(client).ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    _logger.ErrorMessage(task.Exception!.Message);
                }
            },
            TaskScheduler.Default);

        // Must run after AddClientAsync(): it synchronously registers the manager's Disconnected
        // handler, which emits RequestFailed for pending requests (NetworkManager.RemoveClient).
        // ForwardNetworkEvents() registers a later Disconnected handler that stops forwarding, so the
        // opposite order would drop those RequestFailed events.
        ForwardNetworkEvents(netManager);

        _ = client.SendAsync("Runtime.enable").ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    _logger.ErrorMessage(task.Exception!.Message);
                }
            },
            TaskScheduler.Default);

        _ = client.SendAsync("Log.enable").ContinueWith(
            task =>
            {
                if (task.IsFaulted)
                {
                    _logger.ErrorMessage(task.Exception!.Message);
                }
            },
            TaskScheduler.Default);
    }

    /// <inheritdoc/>
    public override ICDPSession Client { get; }

    internal CDPSession CdpCDPSession => (CdpCDPSession)Client;

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
                if (!CdpCDPSession.Detached)
                {
                    await CdpCDPSession.Connection.SendAsync(
                        "Target.closeTarget",
                        new TargetCloseTargetRequest()
                        {
                            TargetId = _id,
                        }).ConfigureAwait(false);

                    await CdpCDPSession.Connection.SendAsync(
                        "Target.detachFromTarget",
                        new TargetDetachFromTargetRequest()
                        {
                            SessionId = Client.Id,
                        }).ConfigureAwait(false);
                }

                break;
            case TargetType.SharedWorker:
                if (!CdpCDPSession.Detached)
                {
                    await CdpCDPSession.Connection.SendAsync(
                        "Target.closeTarget",
                        new TargetCloseTargetRequest()
                        {
                            TargetId = _id,
                        }).ConfigureAwait(false);
                }

                break;
            default:
                await EvaluateFunctionAsync(@"() => {
                        self.close();
                    }").ConfigureAwait(false);
                break;
        }
    }

    /// <inheritdoc/>
    public override async Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
    {
        await _workerScriptLoaded.Task.ConfigureAwait(false);
        return await base.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task EvaluateFunctionAsync(string script, params object[] args)
    {
        await _workerScriptLoaded.Task.ConfigureAwait(false);
        await base.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<T> EvaluateExpressionAsync<T>(string script)
    {
        await _workerScriptLoaded.Task.ConfigureAwait(false);
        return await base.EvaluateExpressionAsync<T>(script).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
    {
        await _workerScriptLoaded.Task.ConfigureAwait(false);
        return await base.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
    {
        // Match evaluate*: wait until the worker script has run so WaitForFunction does not
        // start polling before top-level worker setup (e.g. setTimeout) has been scheduled.
        await _workerScriptLoaded.Task.ConfigureAwait(false);
        return await base.WaitForFunctionAsync(script, options, args).ConfigureAwait(false);
    }

    /// <summary>
    /// Network manager for workers that are not owned by a page (service and shared workers).
    /// It only tracks this worker's session.
    /// </summary>
    private NetworkManager CreateOwnNetworkManager(CdpTarget target, CDPSession client)
        => new(new WorkerFrameProvider(this, target), client.Connection.LoggerFactory, target.Browser.IsNetworkEnabled());

    /// <summary>
    /// Re-emits network events that belong to this worker. The network manager may be shared with a
    /// page (dedicated workers), so events are filtered by <see cref="IRequest.Worker"/>.
    /// </summary>
    private void ForwardNetworkEvents(NetworkManager networkManager)
    {
        void ForwardRequest(object sender, RequestEventArgs e)
        {
            if (IsOwn(e.Request))
            {
                OnRequest(e);
            }
        }

        void ForwardRequestServedFromCache(object sender, RequestEventArgs e)
        {
            if (IsOwn(e.Request))
            {
                OnRequestServedFromCache(e);
            }
        }

        void ForwardResponse(object sender, ResponseCreatedEventArgs e)
        {
            if (IsOwn(e.Response.Request))
            {
                OnResponse(e);
            }
        }

        void ForwardRequestFailed(object sender, RequestEventArgs e)
        {
            if (IsOwn(e.Request))
            {
                OnRequestFailed(e);
            }
        }

        void ForwardRequestFinished(object sender, RequestEventArgs e)
        {
            if (IsOwn(e.Request))
            {
                OnRequestFinished(e);
            }
        }

        networkManager.Request += ForwardRequest;
        networkManager.RequestServedFromCache += ForwardRequestServedFromCache;
        networkManager.Response += ForwardResponse;
        networkManager.RequestFailed += ForwardRequestFailed;
        networkManager.RequestFinished += ForwardRequestFinished;

        void OnDisconnected(object sender, EventArgs e)
        {
            Client.Disconnected -= OnDisconnected;
            networkManager.Request -= ForwardRequest;
            networkManager.RequestServedFromCache -= ForwardRequestServedFromCache;
            networkManager.Response -= ForwardResponse;
            networkManager.RequestFailed -= ForwardRequestFailed;
            networkManager.RequestFinished -= ForwardRequestFinished;
        }

        Client.Disconnected += OnDisconnected;
    }

    /// <summary>
    /// Utility method to check if the request is owned by the current WebWorker.
    /// </summary>
    private bool IsOwn(IRequest request) => request.Worker == this;

    private async void OnMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            switch (e.MessageID)
            {
                case "Runtime.executionContextCreated":
                    OnExecutionContextCreated(e.MessageData.ToObject<RuntimeExecutionContextCreatedResponse>());
                    break;
                case "Runtime.consoleAPICalled":
                    await OnConsoleAPICalledAsync(e).ConfigureAwait(false);
                    break;
                case "Runtime.exceptionThrown":
                    OnExceptionThrown(e.MessageData.ToObject<RuntimeExceptionThrownResponse>());
                    break;
                case "Inspector.workerScriptLoaded":
                    _workerScriptLoaded.TrySetResult(true);
                    break;
            }
        }
        catch (Exception ex)
        {
            var message = $"Worker failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
            _logger.ErrorWithException(ex, message);
            Client.Close(message);
        }
    }

    private void OnExceptionThrown(RuntimeExceptionThrownResponse e)
    {
        var details = e.ExceptionDetails;
        _exceptionThrown(details);

        if (!HasExceptionThrownListeners && !_hasOwnerExceptionListeners())
        {
            // Nobody can use a handle, but the browser keeps the exception alive until it is released.
            if (details.Exception != null)
            {
                _ = RemoteObjectHelper.ReleaseObjectAsync(Client, CdpRemoteException.ToRemoteObject(details.Exception), _logger);
            }

            return;
        }

        var handle = CreateExceptionHandle(details.Exception);
        try
        {
            var exception = CdpRemoteException.Create(details, handle);

            // Forward to the page first, then to the worker listeners.
            _forwardException(exception);
            OnExceptionThrown(new RemoteExceptionEventArgs(exception));
        }
        finally
        {
            // This is a no-op if a listener called Move() on the handle.
            if (handle != null)
            {
                _ = DisposeHandleAsync(handle);
            }
        }
    }

    private async Task DisposeHandleAsync(IJSHandle handle)
    {
        try
        {
            await handle.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.ErrorDisposingExceptionHandle(ex);
        }
    }

    private IJSHandle CreateExceptionHandle(EvaluateExceptionResponseInfo info)
        => string.IsNullOrEmpty(info?.ObjectId)
            ? null
            : new CdpJSHandle(World, CdpRemoteException.ToRemoteObject(info));

    private async Task OnConsoleAPICalledAsync(MessageEventArgs e)
    {
        var consoleData = e.MessageData.ToObject<PageConsoleResponse>();
        var handles = consoleData.Args.Select(i => new CdpJSHandle(World, i)).ToArray<IJSHandle>();

        var tokens = handles.Select(i =>
        {
            var handle = (ICdpHandle)i;
            if (handle.RemoteObject.Subtype == RemoteObjectSubtype.Error && !string.IsNullOrEmpty(handle.RemoteObject.Description))
            {
                return handle.RemoteObject.Description.Split('\n')[0];
            }

            return handle.RemoteObject.ObjectId != null || handle.RemoteObject.Type == RemoteObjectType.Object
                ? i.ToString()
                : RemoteObjectHelper.ValueFromRemoteObject<object>(handle.RemoteObject)?.ToString() ?? "null";
        });

        var location = new ConsoleMessageLocation();
        var stackTraceLocations = new List<ConsoleMessageLocation>();
        if (consoleData.StackTrace?.CallFrames?.Length > 0)
        {
            foreach (var callFrame in consoleData.StackTrace.CallFrames)
            {
                stackTraceLocations.Add(new ConsoleMessageLocation
                {
                    URL = callFrame.URL,
                    LineNumber = callFrame.LineNumber,
                    ColumnNumber = callFrame.ColumnNumber,
                    FunctionName = callFrame.FunctionName,
                    ScriptId = callFrame.ScriptId,
                });
            }

            location = stackTraceLocations[0];
        }

        var consoleMessage = new ConsoleMessage(consoleData.Type, string.Join(" ", tokens), handles, location, stackTraceLocations, worker: this);
        OnConsole(new ConsoleEventArgs(consoleMessage));

        await _consoleAPICalled(consoleData.Type, handles, consoleData.StackTrace).ConfigureAwait(false);
    }

    private void OnExecutionContextCreated(RuntimeExecutionContextCreatedResponse e)
    {
        if (!World.HasContext)
        {
            World.SetNewContext(CdpCDPSession, e.Context, World);
        }
    }

    private sealed class WorkerFrameProvider(CdpWebWorker worker, CdpTarget target) : IFrameProvider
    {
        public IBrowser Browser => target.Browser;

        public Task<CdpFrame> GetFrameAsync(string frameId) => Task.FromResult<CdpFrame>(null);

        public WebWorker Worker(CDPSession client) => client == worker.Client ? worker : null;
    }
}
