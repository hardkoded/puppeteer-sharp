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
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpTarget : Target
{
    private TaskCompletionSource<bool> _closeOperation;

    internal CdpTarget(
        TargetInfo targetInfo,
        CdpCDPSession session,
        CdpBrowserContext context,
        ITargetManager targetManager,
        Func<bool, Task<CDPSession>> sessionFactory,
        TaskQueue screenshotTaskQueue
    )
    {
        TargetInfo = targetInfo;
        Session = session;
        BrowserContext = context;
        ScreenshotTaskQueue = screenshotTaskQueue;
        TargetManager = targetManager;
        SessionFactory = sessionFactory;

        if (session != null)
        {
            session.Target = this;
        }

        Initialize();
    }

    /// <inheritdoc/>
    public override string Url => TargetInfo.Url;

    /// <inheritdoc/>
    public override TargetType Type => TargetInfo.Type;

    /// <summary>
    /// Gets the target identifier.
    /// </summary>
    /// <value>The target identifier.</value>
    public string TargetId => TargetInfo.TargetId;

    /// <inheritdoc/>
    public override ITarget Opener =>
        TargetInfo.OpenerId != null
            ? CdpBrowser.TargetManager.GetAvailableTargets().GetValueOrDefault(TargetInfo.OpenerId)
            : null;

    internal Task<InitializationStatus> InitializedTask => InitializedTaskWrapper.Task;

    internal TaskCompletionSource<InitializationStatus> InitializedTaskWrapper { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal Task CloseTask => CloseTaskWrapper.Task;

    internal TaskCompletionSource<bool> CloseTaskWrapper { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal Func<bool, Task<CDPSession>> SessionFactory { get; private set; }

    internal ITargetManager TargetManager { get; }

    internal bool IsInitialized { get; set; }

    internal override Browser Browser => BrowserContext.Browser;

    internal override BrowserContext BrowserContext { get; }

    internal TargetInfo TargetInfo { get; set; }

    internal CDPSession Session { get; private set; }

    internal CdpBrowser CdpBrowser => Browser as CdpBrowser;

    internal TaskQueue ScreenshotTaskQueue { get; }

    /// <summary>
    /// Guards the state shared between <see cref="CloseAsync"/> and the targets that attach sessions.
    /// </summary>
    private protected object CloseLock { get; } = new();

    /// <summary>
    /// Whether <see cref="CloseAsync"/> is closing the target. Read it while holding <see cref="CloseLock"/>.
    /// </summary>
    private protected bool IsClosing => _closeOperation != null;

    /// <inheritdoc/>
    public override async Task<IPage> AsPageAsync()
    {
        if (Session == null)
        {
            var session = (CdpCDPSession)await CreateCDPSessionAsync().ConfigureAwait(false);
            return await CdpPage
                .CreateAsync(session, this, null, ScreenshotTaskQueue)
                .ConfigureAwait(false);
        }

        return await CdpPage
            .CreateAsync((CdpCDPSession)Session, this, null, ScreenshotTaskQueue)
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<ICDPSession> CreateCDPSessionAsync()
    {
        var session = await SessionFactory(false).ConfigureAwait(false);
        session.Target = this;

        // Worker-type sessions buffer their early method-events until flushed (see
        // CDPSession.BufferEarlyMessages), so an internal consumer that subscribes right after
        // attaching (e.g. CdpWebWorker) doesn't lose Chrome's replayed init events. This public
        // API returns the session before the caller has a chance to subscribe, so there is no
        // "internal listener" to protect here; flush immediately so future events (e.g.
        // Debugger.scriptParsed) reach the caller's MessageReceived handler instead of sitting in
        // the buffer forever.
        session.FlushEarlyMessages();
        return session;
    }

    /// <inheritdoc/>
    public override async Task CloseAsync()
    {
        // Chrome closes the whole page when it closes the target of a frame.
        if (TargetInfo.Type == TargetType.IFrame)
        {
            throw new PuppeteerException("The target of a frame does not support closing");
        }

        if (CloseTask.IsCompleted)
        {
            return;
        }

        // Concurrent calls share one close operation, because a second
        // `Target.closeTarget` fails if the target is already gone.
        TaskCompletionSource<bool> operation;
        var isNewOperation = false;
        lock (CloseLock)
        {
            if (_closeOperation == null)
            {
                _closeOperation = new TaskCompletionSource<bool>(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );
                isNewOperation = true;
            }

            operation = _closeOperation;
        }

        if (isNewOperation)
        {
            _ = CloseInternalAsync(operation);
        }

        await operation.Task.ConfigureAwait(false);
    }

    internal void SetSession(CdpCDPSession session)
    {
        Session = session;
        session.Target = this;
    }

    internal void TargetInfoChanged(TargetInfo targetInfo)
    {
        TargetInfo = targetInfo;
        CheckIfInitialized();
    }

    /// <summary>
    /// Initializes the target.
    /// </summary>
    internal virtual void Initialize()
    {
        IsInitialized = true;
        InitializedTaskWrapper.TrySetResult(InitializationStatus.Success);
    }

    /// <summary>
    /// Returns the sessions that Puppeteer keeps attached to the target.
    /// </summary>
    /// <returns>A task that returns the attached sessions.</returns>
    internal virtual Task<IReadOnlyCollection<CdpCDPSession>> AttachedSessionsAsync() =>
        Task.FromResult(TargetManager.GetAttachedSessions(TargetId));

    /// <summary>
    /// Check is the target is not initialized.
    /// </summary>
    protected internal virtual void CheckIfInitialized()
    {
        IsInitialized = true;
        InitializedTaskWrapper.TrySetResult(InitializationStatus.Success);
    }

    private static async Task DetachAsync(CdpCDPSession session)
    {
        if (session.Detached)
        {
            return;
        }

        // `DetachAsync()` sends the command on the connection, which fails for a session
        // attached through a page. There is no parent connection if the parent session is
        // already gone.
        var parentConnection = session.ParentConnection;
        if (parentConnection == null)
        {
            return;
        }

        try
        {
            await parentConnection
                .SendAsync(
                    "Target.detachFromTarget",
                    new TargetDetachFromTargetRequest { SessionId = session.Id }
                )
                .ConfigureAwait(false);
        }
        catch (Exception) when (session.Detached)
        {
            // Chrome might have detached the session already, for example because
            // its page closed.
        }
    }

    private async Task CloseInternalAsync(TaskCompletionSource<bool> operation)
    {
        try
        {
            await CdpBrowser.CloseTargetAsync(TargetId).ConfigureAwait(false);

            if (TargetInfo.Type == TargetType.ServiceWorker)
            {
                // Chrome reports a closed service worker as destroyed only after the
                // sessions attached to it detach.
                var sessions = await AttachedSessionsAsync().ConfigureAwait(false);
                await Task.WhenAll(sessions.Select(DetachAsync)).ConfigureAwait(false);
            }

            await CloseTask.ConfigureAwait(false);
            EndCloseOperation();
            operation.TrySetResult(true);
        }
        catch (Exception ex)
        {
            EndCloseOperation();
            operation.TrySetException(ex);
        }
    }

    private void EndCloseOperation()
    {
        lock (CloseLock)
        {
            _closeOperation = null;
        }
    }
}
