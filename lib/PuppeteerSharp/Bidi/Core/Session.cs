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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using WebDriverBiDi;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Input;
using WebDriverBiDi.Network;
using WebDriverBiDi.Session;

#if !CDP_ONLY

namespace PuppeteerSharp.Bidi.Core;

internal class Session(BiDiDriver driver, NewCommandResult info) : IDisposable
{
    private readonly ConcurrentSet<WebDriverBiDi.BrowsingContext.NavigationEventArgs> _fragmentSeen = new();

    public event EventHandler<SessionEndArgs> Ended;

    public event EventHandler<BrowsingContextEventArgs> BrowsingContextContextCreated;

    public event EventHandler<BrowsingContextEventArgs> BrowsingContextContextDestroyed;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextNavigationStarted;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextLoad;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextDomContentLoaded;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextNavigationAborted;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextNavigationFailed;

    public event EventHandler<WebDriverBiDi.BrowsingContext.NavigationEventArgs> BrowsingContextFragmentNavigated;

    public event EventHandler<WebDriverBiDi.BrowsingContext.HistoryUpdatedEventArgs> BrowsingContextHistoryUpdated;

    public event EventHandler<BeforeRequestSentEventArgs> NetworkBeforeRequestSent;

    public event EventHandler<AuthRequiredEventArgs> NetworkAuthRequired;

    public event EventHandler<FetchErrorEventArgs> NetworkFetchError;

    public event EventHandler<ResponseCompletedEventArgs> NetworkResponseComplete;

    public event EventHandler<ResponseStartedEventArgs> NetworkResponseStarted;

    public event EventHandler<WebDriverBiDi.BrowsingContext.UserPromptOpenedEventArgs> BrowsingContextUserPromptOpened;

    public event EventHandler<WebDriverBiDi.BrowsingContext.UserPromptClosedEventArgs> BrowsingContextUserPromptClosed;

    public event EventHandler<WebDriverBiDi.Log.EntryAddedEventArgs> LogEntryAdded;

    public event EventHandler<FileDialogOpenedEventArgs> InputFileDialogOpened;

    public BiDiDriver Driver { get; } = driver;

    public NewCommandResult Info { get; } = info;

    public Browser Browser { get; private set; }

    internal ScriptInjector ScriptInjector => ScriptInjector.Default;

    public static async Task<Session> FromAsync(BiDiDriver driver, NewCommandParameters capabilities, ILoggerFactory loggerFactory)
    {
        var result = await driver.Session.NewSessionAsync(capabilities).ConfigureAwait(false);
        var session = new Session(driver, result);
        await session.InitializeAsync().ConfigureAwait(false);
        return session;
    }

    public void Dispose()
    {
    }

    public async Task SubscribeAsync(string[] events, string[] contexts = null)
    {
        var args = new SubscribeCommandParameters();
        args.Events.AddRange(events);
        args.Contexts.AddRange(contexts ?? []);
        await Driver.Session.SubscribeAsync(args).ConfigureAwait(false);
    }

    internal virtual void OnEnded(SessionEndArgs e) => Ended?.Invoke(this, e);

    internal virtual void OnBrowsingContextContextCreated(BrowsingContextEventArgs e) => BrowsingContextContextCreated?.Invoke(this, e);

    private async Task InitializeAsync()
    {
        Browser = await Browser.From(this).ConfigureAwait(false);
        Driver.BrowsingContext.OnContextCreated.AddObserver(OnBrowsingContextContextCreated);
        Driver.BrowsingContext.OnContextDestroyed.AddObserver(OnBrowsingContextContextDestroyed);
        Driver.BrowsingContext.OnDomContentLoaded.AddObserver(OnBrowsingContextDomContentLoaded);
        Driver.BrowsingContext.OnLoad.AddObserver(OnBrowsingContextLoad);
        Driver.BrowsingContext.OnNavigationStarted.AddObserver(OnBrowsingContextNavigationStarted);
        Driver.BrowsingContext.OnFragmentNavigated.AddObserver(OnFragmentNavigated);
        Driver.BrowsingContext.OnNavigationFailed.AddObserver(OnBrowsingContextNavigationFailed);
        Driver.BrowsingContext.OnNavigationAborted.AddObserver(OnBrowsingContextNavigationAborted);
        Driver.BrowsingContext.OnHistoryUpdated.AddObserver(OnBrowsingContextHistoryUpdated);
        Driver.Network.OnBeforeRequestSent.AddObserver(OnBeforeRequestSent);
        Driver.Network.OnAuthRequired.AddObserver(OnNetworkAuthRequired);
        Driver.Network.OnFetchError.AddObserver(OnNetworkFetchError);
        Driver.Network.OnResponseStarted.AddObserver(OnNetworkResponseStarted);
        Driver.Network.OnResponseCompleted.AddObserver(OnNetworkResponseCompleted);
        Driver.BrowsingContext.OnUserPromptOpened.AddObserver(OnBrowsingContextUserPromptOpened);
        Driver.BrowsingContext.OnUserPromptClosed.AddObserver(OnBrowsingContextUserPromptClosed);
        Driver.Log.OnEntryAdded.AddObserver(OnLogEntryAdded);
        Driver.Input.OnFileDialogOpened.AddObserver(OnInputFileDialogOpened);
    }

    private void OnFragmentNavigated(WebDriverBiDi.BrowsingContext.NavigationEventArgs info)
    {
        if (_fragmentSeen.Contains(info))
        {
            return;
        }

        _fragmentSeen.Add(info);
        OnBrowsingContextNavigationStarted(info);
        OnBrowsingContextFragmentNavigated(info);
    }

    private void OnNetworkResponseCompleted(ResponseCompletedEventArgs obj) => NetworkResponseComplete?.Invoke(this, obj);

    private void OnNetworkResponseStarted(ResponseStartedEventArgs obj) => NetworkResponseStarted?.Invoke(this, obj);

    private void OnNetworkFetchError(FetchErrorEventArgs obj) => NetworkFetchError?.Invoke(this, obj);

    private void OnNetworkAuthRequired(AuthRequiredEventArgs obj) => NetworkAuthRequired?.Invoke(this, obj);

    private void OnBeforeRequestSent(BeforeRequestSentEventArgs obj) => NetworkBeforeRequestSent?.Invoke(this, obj);

    private void OnBrowsingContextNavigationAborted(WebDriverBiDi.BrowsingContext.NavigationEventArgs obj) => BrowsingContextNavigationAborted?.Invoke(this, obj);

    private void OnBrowsingContextNavigationFailed(WebDriverBiDi.BrowsingContext.NavigationEventArgs obj) => BrowsingContextNavigationFailed?.Invoke(this, obj);

    private void OnBrowsingContextFragmentNavigated(WebDriverBiDi.BrowsingContext.NavigationEventArgs obj) => BrowsingContextFragmentNavigated?.Invoke(this, obj);

    private void OnBrowsingContextHistoryUpdated(WebDriverBiDi.BrowsingContext.HistoryUpdatedEventArgs obj) => BrowsingContextHistoryUpdated?.Invoke(this, obj);

    private void OnBrowsingContextNavigationStarted(WebDriverBiDi.BrowsingContext.NavigationEventArgs obj) => BrowsingContextNavigationStarted?.Invoke(this, obj);

    private void OnBrowsingContextLoad(WebDriverBiDi.BrowsingContext.NavigationEventArgs arg) => BrowsingContextLoad?.Invoke(this, arg);

    private void OnBrowsingContextDomContentLoaded(WebDriverBiDi.BrowsingContext.NavigationEventArgs obj) => BrowsingContextDomContentLoaded?.Invoke(this, obj);

    private void OnBrowsingContextContextDestroyed(BrowsingContextEventArgs obj) => BrowsingContextContextDestroyed?.Invoke(this, obj);

    private void OnBrowsingContextUserPromptOpened(WebDriverBiDi.BrowsingContext.UserPromptOpenedEventArgs obj) => BrowsingContextUserPromptOpened?.Invoke(this, obj);

    private void OnBrowsingContextUserPromptClosed(WebDriverBiDi.BrowsingContext.UserPromptClosedEventArgs obj) => BrowsingContextUserPromptClosed?.Invoke(this, obj);

    private void OnLogEntryAdded(WebDriverBiDi.Log.EntryAddedEventArgs obj) => LogEntryAdded?.Invoke(this, obj);

    private void OnInputFileDialogOpened(FileDialogOpenedEventArgs obj) => InputFileDialogOpened?.Invoke(this, obj);
}

#endif
