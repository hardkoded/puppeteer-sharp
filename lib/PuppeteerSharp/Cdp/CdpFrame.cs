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
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpFrame : Frame
{
    private const string RefererHeaderName = "referer";

    internal CdpFrame(FrameManager frameManager, string frameId, string parentFrameId, CDPSession client)
    {
        FrameManager = frameManager;
        Id = frameId;
        Client = client;
        ParentId = parentFrameId;

        UpdateClient(client);

        FrameSwappedByActivation += (_, _) =>
        {
            // Emulate loading process for swapped frames.
            OnLoadingStarted();
            OnLoadingStopped();
        };

        Logger = client.Connection.LoggerFactory.CreateLogger<Frame>();
    }

    /// <inheritdoc />
    public override CDPSession Client { get; protected set; }

    /// <inheritdoc/>
    public override IPage Page => FrameManager.Page;

    /// <inheritdoc/>
    public override bool IsOopFrame => Client != FrameManager.Client;

    /// <inheritdoc/>
    public override IReadOnlyCollection<IFrame> ChildFrames => FrameManager.FrameTree.GetChildFrames(Id);

    internal FrameManager FrameManager { get; }

    internal override Frame ParentFrame => FrameManager.FrameTree.GetParentFrame(Id);

    /// <inheritdoc/>
    public override async Task<IResponse> GoToAsync(string url, NavigationOptions options)
    {
        var ensureNewDocumentNavigation = false;

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var referrer = string.IsNullOrEmpty(options.Referer)
            ? FrameManager.NetworkManager.ExtraHTTPHeaders?.GetValueOrDefault(RefererHeaderName)
            : options.Referer;
        var referrerPolicy = string.IsNullOrEmpty(options.ReferrerPolicy)
            ? FrameManager.NetworkManager.ExtraHTTPHeaders?.GetValueOrDefault("referer-policy")
            : options.ReferrerPolicy;
        var timeout = options.Timeout ?? FrameManager.TimeoutSettings.NavigationTimeout;

        using var watcher = new LifecycleWatcher(FrameManager.NetworkManager, this, options.WaitUntil, timeout);
        try
        {
            var navigateTask = NavigateAsync();
            var task = await Task.WhenAny(
                watcher.TerminationTask,
                navigateTask).ConfigureAwait(false);

            await task.ConfigureAwait(false);

            task = await Task.WhenAny(
                watcher.TerminationTask,
                ensureNewDocumentNavigation ? watcher.NewDocumentNavigationTask : watcher.SameDocumentNavigationTask).ConfigureAwait(false);

            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new NavigationException(ex.Message, ex);
        }

        return watcher.NavigationResponse;

        async Task NavigateAsync()
        {
            var response = await Client.SendAsync<PageNavigateResponse>("Page.navigate", new PageNavigateRequest
            {
                Url = url,
                Referrer = referrer ?? string.Empty,
                ReferrerPolicy = referrerPolicy ?? string.Empty,
                FrameId = Id,
            }).ConfigureAwait(false);

            ensureNewDocumentNavigation = !string.IsNullOrEmpty(response.LoaderId);

            if (!string.IsNullOrEmpty(response.ErrorText) && response.ErrorText != "net::ERR_HTTP_RESPONSE_CODE_FAILURE")
            {
                throw new NavigationException(response.ErrorText, url);
            }
        }
    }

    /// <inheritdoc/>
    public override async Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
    {
        var timeout = options?.Timeout ?? FrameManager.TimeoutSettings.NavigationTimeout;
        using var watcher = new LifecycleWatcher(FrameManager.NetworkManager, this, options?.WaitUntil, timeout);
        var raceTask = await Task.WhenAny(
            watcher.NewDocumentNavigationTask,
            watcher.SameDocumentNavigationTask,
            watcher.TerminationTask).ConfigureAwait(false);

        await raceTask.ConfigureAwait(false);

        return watcher.NavigationResponse;
    }

    /// <inheritdoc/>
    public override async Task SetContentAsync(string html, NavigationOptions options = null)
    {
        var waitUntil = options?.WaitUntil ?? new[] { WaitUntilNavigation.Load };
        var timeout = options?.Timeout ?? FrameManager.TimeoutSettings.NavigationTimeout;

        // We rely upon the fact that document.open() will reset frame lifecycle with "init"
        // lifecycle event. @see https://crrev.com/608658
        await IsolatedRealm.EvaluateFunctionAsync(
            @"html => {
                    document.open();
                    document.write(html);
                    document.close();
                }",
            html).ConfigureAwait(false);

        using var watcher = new LifecycleWatcher(FrameManager.NetworkManager, this, waitUntil, timeout);
        var watcherTask = await Task.WhenAny(
            watcher.TerminationTask,
            watcher.LifecycleTask).ConfigureAwait(false);

        await watcherTask.ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) && string.IsNullOrEmpty(options.Content))
        {
            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        var content = options.Content;

        if (!string.IsNullOrEmpty(options.Path))
        {
            content = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
            content += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
        }

        var handle = await IsolatedRealm.EvaluateFunctionHandleAsync(
            @"async (puppeteerUtil, url, id, type, content) => {
                  const createDeferredPromise = puppeteerUtil.createDeferredPromise;
                  const promise = createDeferredPromise();
                  let element;
                  if (!url) {
                    element = document.createElement('style');
                    element.appendChild(document.createTextNode(content));
                  } else {
                    const link = document.createElement('link');
                    link.rel = 'stylesheet';
                    link.href = url;
                    element = link;
                  }
                  element.addEventListener(
                    'load',
                    () => {
                      promise.resolve();
                    },
                    {once: true}
                  );
                  element.addEventListener(
                    'error',
                    event => {
                      promise.reject(
                        new Error(
                          event.message ?? 'Could not load style'
                        )
                      );
                    },
                    {once: true}
                  );
                  document.head.appendChild(element);
                  await promise;
                  return element;
                }",
            new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
            options.Url,
            options.Id,
            options.Type,
            content).ConfigureAwait(false);

        return (await MainRealm.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
    }

    /// <inheritdoc/>
    public override async Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrEmpty(options.Url) && string.IsNullOrEmpty(options.Path) && string.IsNullOrEmpty(options.Content))
        {
            throw new ArgumentException("Provide options with a `Url`, `Path` or `Content` property");
        }

        var content = options.Content;

        if (!string.IsNullOrEmpty(options.Path))
        {
            content = await AsyncFileHelper.ReadAllText(options.Path).ConfigureAwait(false);
            content += "//# sourceURL=" + options.Path.Replace("\n", string.Empty);
        }

        var handle = await IsolatedRealm.EvaluateFunctionHandleAsync(
            @"async (puppeteerUtil, url, id, type, content) => {
                  const createDeferredPromise = puppeteerUtil.createDeferredPromise;
                  const promise = createDeferredPromise();
                  const script = document.createElement('script');
                  script.type = type;
                  script.text = content;
                  if (url) {
                    script.src = url;
                    script.addEventListener(
                      'load',
                      () => {
                        return promise.resolve();
                      },
                      {once: true}
                    );
                    script.addEventListener(
                      'error',
                      event => {
                        promise.reject(
                          new Error(event.message ?? 'Could not load script')
                        );
                      },
                      {once: true}
                    );
                  } else {
                    promise.resolve();
                  }
                  if (id) {
                    script.id = id;
                  }
                  document.head.appendChild(script);
                  await promise;
                  return script;
                }",
            new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
            options.Url,
            options.Id,
            options.Type,
            content).ConfigureAwait(false);

        return (await MainRealm.TransferHandleAsync(handle).ConfigureAwait(false)) as IElementHandle;
    }

    internal void UpdateClient(CDPSession client, bool keepWorlds = false)
    {
        Client = client;

        if (!keepWorlds)
        {
            MainWorld?.ClearContext();
            PuppeteerWorld?.ClearContext();

            MainRealm = new IsolatedWorld(
                this,
                null,
                FrameManager.TimeoutSettings,
                true);

            IsolatedRealm = new IsolatedWorld(
                this,
                null,
                FrameManager.TimeoutSettings,
                false);
        }
        else
        {
            MainWorld.FrameUpdated();
            PuppeteerWorld.FrameUpdated();
        }
    }

    /// <inheritdoc />
    protected internal override DeviceRequestPromptManager GetDeviceRequestPromptManager()
    {
        if (IsOopFrame)
        {
            return FrameManager.GetDeviceRequestPromptManager(Client);
        }

        if (ParentFrame == null)
        {
            throw new PuppeteerException("Unable to find parent frame");
        }

        return ParentFrame.GetDeviceRequestPromptManager();
    }
}
