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

#if !CDP_ONLY

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Emulation;
using WebDriverBiDi.Input;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi.Core;

internal class BrowsingContext : IDisposable
{
    private readonly ConcurrentDictionary<string, BrowsingContext> _children = new();
    private readonly List<string> _childrenOrder = new();
    private readonly object _childrenLock = new();
    private readonly ConcurrentDictionary<string, Request> _requests = new();
    private string _reason;
    private Navigation _navigation;

    private BrowsingContext(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener, string clientWindow)
    {
        UserContext = userContext;
        Parent = parent;
        Id = id;
        Url = url;
        OriginalOpener = originalOpener;
        WindowId = clientWindow;

        DefaultRealm = CreateWindowRealm();
    }

    public event EventHandler<ClosedEventArgs> Closed;

    public event EventHandler<WorkerRealmEventArgs> Worker;

    public event EventHandler DomContentLoaded;

    public event EventHandler Load;

    public event EventHandler<BidiBrowsingContextEventArgs> BrowsingContextCreated;

    public event EventHandler<RequestEventArgs> Request;

    public event EventHandler<BrowserContextNavigationEventArgs> Navigation;

    public event EventHandler HistoryUpdated;

    public event EventHandler<UserPromptEventArgs> UserPrompt;

    public event EventHandler<FileDialogOpenedEventArgs> FileDialogOpened;

    public event EventHandler<WebDriverBiDi.Log.EntryAddedEventArgs> Log;

    public UserContext UserContext { get; }

    public string Id { get; }

    public string Url { get; private set; }

    public bool IsClosed { get; private set; }

    public Session Session => UserContext.Browser.Session;

    public IEnumerable<BrowsingContext> Children
    {
        get
        {
            lock (_childrenLock)
            {
                var result = new List<BrowsingContext>(_childrenOrder.Count);
                foreach (var id in _childrenOrder)
                {
                    if (_children.TryGetValue(id, out var child))
                    {
                        result.Add(child);
                    }
                }

                return result;
            }
        }
    }

    public WindowRealm DefaultRealm { get; }

    public string WindowId { get; }

    internal string OriginalOpener { get; }

    internal BrowsingContext Top
    {
        get
        {
            var context = this;

            while (context.Parent != null)
            {
                context = context.Parent;
            }

            return context;
        }
    }

    private BrowsingContext Parent { get; }

    public static BrowsingContext From(UserContext userContext, BrowsingContext parent, string id, string url, string originalOpener, string clientWindow = null)
    {
        var context = new BrowsingContext(userContext, parent, id, url, originalOpener, clientWindow);
        context.Initialize();
        return context;
    }

    public void Dispose()
    {
        if (IsClosed)
        {
            return;
        }

        IsClosed = true;

        _reason ??= "Browser was disconnected, probably because the session ended.";
        OnClosed(_reason);
        foreach (var context in _children.Values)
        {
            context.Dispose("Parent browsing context was disposed");
        }
    }

    public async Task CloseAsync(bool? promptUnload)
    {
        foreach (var context in _children.Values)
        {
            await context.CloseAsync(promptUnload).ConfigureAwait(false);
        }

        await Session.Driver.BrowsingContext.CloseAsync(new CloseCommandParameters(Id)
        {
            PromptUnload = promptUnload,
        }).ConfigureAwait(false);
    }

    internal async Task NavigateAsync(string url, ReadinessState wait)
    {
        await Session.Driver.BrowsingContext.NavigateAsync(new NavigateCommandParameters(Id, url)
        {
            Wait = wait,
        }).ConfigureAwait(false);
    }

    internal async Task ActivateAsync()
    {
        await Session.Driver.BrowsingContext.ActivateAsync(new ActivateCommandParameters(Id)).ConfigureAwait(false);
    }

    internal async Task<string> CaptureScreenshotAsync(ScreenshotParameters options)
    {
        var parameters = new CaptureScreenshotCommandParameters(Id)
        {
            Format = options.Format,
            Clip = options.Clip,
            Origin = options.Origin,
        };

        return (await Session.Driver.BrowsingContext.CaptureScreenshotAsync(parameters).ConfigureAwait(false)).Data;
    }

    internal async Task<string> PrintAsync(PrintCommandParameters options)
    {
        options.BrowsingContextId = Id;
        return (await Session.Driver.BrowsingContext.PrintAsync(options).ConfigureAwait(false)).Data;
    }

    internal async Task SetViewportAsync(SetViewportOptions options = null)
    {
        var parameters = new SetViewportCommandParameters()
        {
            BrowsingContextId = Id,
            Viewport = options?.Viewport != null
                ? new Viewport() { Width = options.Viewport.Width, Height = options.Viewport.Height }
                : null,
            DevicePixelRatio = options?.DevicePixelRatio,
        };

        await Session.Driver.BrowsingContext.SetViewportAsync(parameters).ConfigureAwait(false);
    }

    internal async Task PerformActionsAsync(SourceActions[] actions)
    {
        var param = new PerformActionsCommandParameters(Id);
        param.Actions.AddRange(actions);
        await Session.Driver.Input.PerformActionsAsync(param).ConfigureAwait(false);
    }

    internal async Task ReleaseActionsAsync()
    {
        await Session.Driver.Input.ReleaseActionsAsync(new ReleaseActionsCommandParameters(Id)).ConfigureAwait(false);
    }

    internal WindowRealm CreateWindowRealm(string sandbox = null)
    {
        var realm = WindowRealm.From(this, sandbox);

        realm.Worker += (_, args) =>
        {
            OnWorker(args.Realm);
        };

        return realm;
    }

    internal async Task TraverseHistoryAsync(int delta)
        => await Session.Driver.BrowsingContext.TraverseHistoryAsync(new TraverseHistoryCommandParameters(Id, delta)).ConfigureAwait(false);

    internal async Task ReloadAsync(bool? ignoreCache = null)
    {
        var parameters = new ReloadCommandParameters(Id)
        {
            IgnoreCache = ignoreCache,
        };
        await Session.Driver.BrowsingContext.ReloadAsync(parameters).ConfigureAwait(false);
    }

    internal async Task<string> AddInterceptAsync(WebDriverBiDi.Network.AddInterceptCommandParameters options)
    {
        options.BrowsingContextIds ??= new List<string>();
        options.BrowsingContextIds.Add(Id);
        var result = await Session.Driver.Network.AddInterceptAsync(options).ConfigureAwait(false);
        return result.InterceptId;
    }

    internal async Task SetUserAgentAsync(string userAgent)
    {
        var parameters = new SetUserAgentOverrideCommandParameters
        {
            UserAgent = userAgent,
            Contexts = [Id],
        };
        await Session.Driver.Emulation.SetUserAgentOverrideAsync(parameters).ConfigureAwait(false);
    }

    internal async Task SetOfflineModeAsync(bool enabled)
    {
        var parameters = new SetNetworkConditionsCommandParameters
        {
            NetworkConditions = enabled ? new NetworkConditionsOffline() : null,
            Contexts = [Id],
        };
        await Session.Driver.Emulation.SetNetworkConditions(parameters).ConfigureAwait(false);
    }

    internal async Task SetScreenOrientationOverrideAsync(ScreenOrientation screenOrientation)
    {
        var parameters = new SetScreenOrientationOverrideCommandParameters
        {
            ScreenOrientation = screenOrientation,
            Contexts = [Id],
        };
        await Session.Driver.Emulation.SetScreenOrientationOverrideAsync(parameters).ConfigureAwait(false);
    }

    internal async Task SetFilesAsync(WebDriverBiDi.Script.SharedReference element, string[] files)
    {
        var parameters = new SetFilesCommandParameters(Id, element);
        foreach (var file in files)
        {
            parameters.Files.Add(file);
        }

        await Session.Driver.Input.SetFilesAsync(parameters).ConfigureAwait(false);
    }

    internal async Task<IList<RemoteValue>> LocateNodesAsync(Locator locator, SharedReference[] startNodes = null)
    {
        var parameters = new LocateNodesCommandParameters(Id, locator);
        if (startNodes?.Length > 0)
        {
            foreach (var node in startNodes)
            {
                parameters.StartNodes.Add(node);
            }
        }

        var result = await Session.Driver.BrowsingContext.LocateNodesAsync(parameters).ConfigureAwait(false);
        return result.Nodes.ToList();
    }

    protected virtual void OnBrowsingContextCreated(BidiBrowsingContextEventArgs e) => BrowsingContextCreated?.Invoke(this, e);

    private void Initialize()
    {
        UserContext.Closed += (_, _) => Dispose("User context was closed");

        Session.BrowsingContextContextCreated += (sender, args) =>
        {
            if (args.Parent != Id)
            {
                return;
            }

            var browsingContext = From(UserContext, this, args.BrowsingContextId, args.Url, args.OriginalOpener);

            lock (_childrenLock)
            {
                _children.TryAdd(browsingContext.Id, browsingContext);
                _childrenOrder.Add(browsingContext.Id);
            }

            browsingContext.Closed += (_, _) =>
            {
                lock (_childrenLock)
                {
                    _children.TryRemove(browsingContext.Id, out _);
                    _childrenOrder.Remove(browsingContext.Id);
                }
            };

            OnBrowsingContextCreated(new BidiBrowsingContextEventArgs(browsingContext));
        };

        Session.BrowsingContextContextDestroyed += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            Dispose("Browsing context already closed.");
        };

        Session.BrowsingContextDomContentLoaded += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            Url = args.Url;
            OnDomContentLoaded();
        };

        Session.BrowsingContextLoad += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            Url = args.Url;
            OnLoad();
        };

        Session.BrowsingContextNavigationStarted += (sender, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            foreach (var entry in _requests)
            {
                if (entry.Value.IsDisposed)
                {
                    _requests.TryRemove(entry.Key, out _);
                }
            }

            // Dispose old navigation if exists - a new navigation has started
            _navigation?.Dispose();

            _navigation = Core.Navigation.From(this);

            _navigation.Fragment += UpdateUrlFromEvent;
            _navigation.Aborted += UpdateUrlFromEvent;
            _navigation.Failed += UpdateUrlFromEvent;

            OnNavigation(new BrowserContextNavigationEventArgs(_navigation));
        };

        Session.BrowsingContextHistoryUpdated += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            // Update URL when history is updated (e.g., via history.pushState/replaceState)
            Url = args.Url;
            OnHistoryUpdated();
        };

        Session.NetworkBeforeRequestSent += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            if (_requests.ContainsKey(args.Request.RequestId))
            {
                return;
            }

            var request = Core.Request.From(this, args);
            _requests.TryAdd(args.Request.RequestId, request);
            Request?.Invoke(this, new RequestEventArgs(request));
        };

        Session.BrowsingContextUserPromptOpened += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            var userPrompt = Core.UserPrompt.From(this, args);
            OnUserPromptOpened(new UserPromptEventArgs(userPrompt));
        };

        Session.LogEntryAdded += (_, args) =>
        {
            if (args.Source.Context != Id)
            {
                return;
            }

            OnLogEntry(args);
        };

        Session.InputFileDialogOpened += (_, args) =>
        {
            if (args.BrowsingContextId != Id)
            {
                return;
            }

            OnFileDialogOpened(args);
        };
    }

    private void OnNavigation(BrowserContextNavigationEventArgs args) => Navigation?.Invoke(this, args);

    private void UpdateUrlFromEvent(object sender, NavigationEventArgs e)
    {
        Url = e.Url;
    }

    private void OnLoad() => Load?.Invoke(this, EventArgs.Empty);

    private void OnDomContentLoaded() => DomContentLoaded?.Invoke(this, EventArgs.Empty);

    private void OnHistoryUpdated() => HistoryUpdated?.Invoke(this, EventArgs.Empty);

    private void Dispose(string reason)
    {
        _reason = reason;
        Dispose();
    }

    private void OnClosed(string reason) => Closed?.Invoke(this, new ClosedEventArgs(reason));

    private void OnWorker(DedicatedWorkerRealm args) => Worker?.Invoke(this, new WorkerRealmEventArgs(args));

    private void OnUserPromptOpened(UserPromptEventArgs args) => UserPrompt?.Invoke(this, args);

    private void OnLogEntry(WebDriverBiDi.Log.EntryAddedEventArgs args) => Log?.Invoke(this, args);

    private void OnFileDialogOpened(FileDialogOpenedEventArgs args) => FileDialogOpened?.Invoke(this, args);
}

#endif
