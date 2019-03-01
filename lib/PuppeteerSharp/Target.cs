using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url} - {_currentState}")]
    public class Target : IStateMachine<Target.State>
    {
        #region Private members

        private State _currentState = State.Initial;
        private TargetInfo _targetInfo;
        private readonly Func<TargetInfo, Task<CDPSession>> _sessionFactory;
        private readonly TaskCompletionSource<bool> _initializeTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private Task<Page> _pageTask;
        private readonly TaskCompletionSource<bool> _closeTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        #endregion

        #region Constructor(s)

        internal Target(TargetInfo targetInfo, Func<TargetInfo, Task<CDPSession>> sessionFactory, BrowserContext browserContext)
        {
            BrowserContext = browserContext;
            _sessionFactory = sessionFactory;
            _currentState.SetTargetInfo(this, targetInfo);

            _ = AfterInitializeAsync();
            async Task<bool> AfterInitializeAsync()
            {
                var success = await _initializeTcs.Task;
                if (success)
                {
                    if (Type == TargetType.Page)
                    {
                        var openerPageTask = Opener?.PageAsync();
                        if (openerPageTask != null)
                        {
                            var openerPage = await openerPageTask.ConfigureAwait(false);
                            if (openerPage.HasPopupEventListeners)
                            {
                                var popupPage = await PageAsync().ConfigureAwait(false);
                                openerPage.OnPopup(popupPage);
                            }
                        }
                    }
                }
                return success;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url => _targetInfo?.Url;
        /// <summary>
        /// Gets the type. It will be <see cref="TargetInfo.Type"/> if it's "page" or "service_worker". Otherwise it will be "other"
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type => _targetInfo?.Type ?? 0;

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId => _targetInfo?.TargetId;

        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        /// <remarks>
        /// Top-level targets return <c>null</c>.
        /// </remarks>
        public Target Opener => _targetInfo?.OpenerId != null ? Browser.TargetById(_targetInfo.OpenerId) : null;

        /// <summary>
        /// Get the browser the target belongs to.
        /// </summary>
        public Browser Browser => BrowserContext?.Browser;

        /// <summary>
        /// Get the browser context the target belongs to.
        /// </summary>
        public BrowserContext BrowserContext { get; }

        internal bool IsInitialized => _initializeTcs.Task.IsCompleted && _initializeTcs.Task.Result;

        private Task<bool> InitializeTask => _initializeTcs.Task;

        internal Task CloseTask => _closeTcs.Task;

        #endregion

        #region Public methods

        /// <summary>
        /// If the target is not of type "page" or "background_page", returns null.
        /// </summary>
        public Task<Page> PageAsync() => _pageTask ?? (_pageTask = CreatePageAsync());

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="CDPSession"/></returns>
        public Task<CDPSession> CreateCDPSessionAsync() =>_sessionFactory(_targetInfo);

        #endregion

        #region Internal methods

        internal void TargetInfoChanged(TargetInfo targetInfo) => _currentState.SetTargetInfo(this, targetInfo);

        internal Task CloseAsync() => _currentState.CloseAsync(this);

        internal void Destroyed() => _currentState.Destroyed(this);

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a new <see cref="Page"/>. If the target is not <c>"page"</c> or <c>"background_page"</c> returns <c>null</c>
        /// </summary>
        /// <returns>a task that returns a new <see cref="Page"/></returns>
        private async Task<Page> CreatePageAsync()
        {
            if (!await _initializeTcs.Task)
            {
                return null;
            }

            switch (Type)
            {
                case TargetType.Page:
                case TargetType.BackgroundPage:
                    var session = await _sessionFactory(_targetInfo)
                        .ConfigureAwait(false);
                    var browser = Browser;
                    return await Page.CreateAsync(session, this, browser.IgnoreHTTPSErrors, browser.DefaultViewport, browser.ScreenshotTaskQueue)
                        .ConfigureAwait(false);
                default:
                    return null;
            }
        }

        private void SetTargetInfoCore(TargetInfo targetInfo, bool notifyBrowser)
        {
            notifyBrowser &= _targetInfo != null && _targetInfo.Url != targetInfo.Url;
            _targetInfo = targetInfo;
            if (notifyBrowser)
            {
                Browser.NotifyTargetChanged(this);
            }
        }

        #endregion

        #region State pattern

        State IStateMachine<State>.CurrentState => _currentState;

        bool IStateMachine<State>.TryEnter(State newState, State fromState)
            => Interlocked.CompareExchange(ref _currentState, newState, fromState) == fromState;

        internal abstract class State : AbstractState<Target, State>
        {
            public static readonly State Initial = new InitialState();
            private static readonly InitializingState Initializing = new InitializingState();
            private static readonly InitializedState Initialized = new InitializedState();
            private static readonly ClosingState Closing = new ClosingState();
            private static readonly ClosedState Closed = new ClosedState();

            public virtual void SetTargetInfo(Target target, TargetInfo targetInfo) => throw InvalidOperation();
            public virtual Task CloseAsync(Target target) => Closing.EnterFromAsync(target, this);
            public virtual void Destroyed(Target target) => Closed.EnterFrom(target, this);

            private class InitialState : State
            {
                public override void SetTargetInfo(Target target, TargetInfo targetInfo) =>
                    Initializing.EnterFrom(target, this, targetInfo);
            }

            private class InitializingState : State
            {
                public void EnterFrom(Target target, State fromState, TargetInfo targetInfo)
                {
                    if (!TryEnter(target, fromState))
                    {
                        target._currentState.SetTargetInfo(target, targetInfo);
                        return;
                    }

                    target.SetTargetInfoCore(targetInfo, false);
                    if (targetInfo.Type != TargetType.Page || targetInfo.Url != string.Empty)
                    {
                        Initialized.EnterFrom(target, this);
                    }
                }

                public override void SetTargetInfo(Target target, TargetInfo targetInfo) => EnterFrom(target, this, targetInfo);
            }

            private class InitializedState : State
            {
                public void EnterFrom(Target target, State fromState)
                {
                    if (TryEnter(target, fromState))
                    {
                        target.Browser.NotifyTargetCreated(target);
                        target._initializeTcs.TrySetResult(true);
                    }
                }

                public override void SetTargetInfo(Target target, TargetInfo targetInfo) => target.SetTargetInfoCore(targetInfo, true);

                public override void Destroyed(Target target) => Closed.EnterFrom(target, this);
            }

            private class ClosingState : State
            {
                public async Task EnterFromAsync(Target target, State fromState)
                {
                    if (!TryEnter(target, fromState))
                    {
                        target._currentState.Destroyed(target);
                    }

                    try
                    {
                        if (await target.InitializeTask.ConfigureAwait(false))
                        {
                            await target.Browser.Connection.SendAsync("Target.closeTarget",
                                    new TargetCloseTargetRequest { TargetId = target.TargetId })
                                .ConfigureAwait(false);
                            await target._closeTcs.Task
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            Closed.EnterFrom(target, this);
                        }
                    }
                    catch (Exception)
                    {
                        Closed.EnterFrom(target, this);
                        throw;
                    }
                }

                public override void SetTargetInfo(Target target, TargetInfo targetInfo) => target.SetTargetInfoCore(targetInfo, target.IsInitialized);
            }

            private class ClosedState : State
            {
                public void EnterFrom(Target target, State fromState)
                {
                    if (!TryEnter(target, fromState))
                    {
                        target._currentState.Destroyed(target);
                    }

                    target._initializeTcs.TrySetResult(false);
                    target._closeTcs.SetResult(true);

                    _ = NotifyTargetDestroyedAsync(target);
                }

                public override Task CloseAsync(Target target) => Task.CompletedTask;

                public override void Destroyed(Target target)
                { }

                private async Task NotifyTargetDestroyedAsync(Target target)
                {
                    if (await target.InitializeTask.ConfigureAwait(false))
                    {
                        // Only notify target destruction if target initialized successfully in the first place.
                        target.Browser.NotifyTargetDestroyed(target);
                    }
                }
            }
        }

        #endregion
    }
}