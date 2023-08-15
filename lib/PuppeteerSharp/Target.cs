using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target : ITarget
    {
        private readonly Func<bool, Task<CDPSession>> _sessionFactory;
        private readonly bool _ignoreHTTPSErrors;
        private readonly ViewPortOptions _defaultViewport;
        private readonly TaskQueue _screenshotTaskQueue;
        private readonly Func<TargetInfo, bool> _isPageTargetFunc;
        private Task<Worker> _workerTask;

        internal Target(
            TargetInfo targetInfo,
            CDPSession session,
            BrowserContext context,
            ITargetManager targetManager,
            Func<bool, Task<CDPSession>> sessionFactory,
            bool ignoreHTTPSErrors,
            ViewPortOptions defaultViewport,
            TaskQueue screenshotTaskQueue,
            Func<TargetInfo, bool> isPageTargetFunc)
        {
            Session = session;
            TargetInfo = targetInfo;
            _isPageTargetFunc = isPageTargetFunc;
            _sessionFactory = sessionFactory;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _defaultViewport = defaultViewport;
            _screenshotTaskQueue = screenshotTaskQueue;
            BrowserContext = context;
            PageTask = null;
            TargetManager = targetManager;

            _ = InitializedTaskWrapper.Task.ContinueWith(
                async initializedTask =>
                {
                    var success = initializedTask.Result;
                    if (!success)
                    {
                        return;
                    }

                    var opener = Opener as Target;

                    var openerPageTask = opener?.PageTask;
                    if (openerPageTask == null || Type != TargetType.Page)
                    {
                        return;
                    }

                    var openerPage = (Page)await openerPageTask.ConfigureAwait(false);
                    if (!openerPage.HasPopupEventListeners)
                    {
                        return;
                    }

                    var popupPage = await PageAsync().ConfigureAwait(false);
                    openerPage.OnPopup(popupPage);
                },
                TaskScheduler.Default);

            IsInitialized = !_isPageTargetFunc(TargetInfo) || !string.IsNullOrEmpty(TargetInfo.Url);

            if (IsInitialized)
            {
                InitializedTaskWrapper.TrySetResult(true);
            }
        }

        /// <inheritdoc/>
        public string Url => TargetInfo.Url;

        /// <inheritdoc/>
        public TargetType Type => TargetInfo.Type;

        /// <inheritdoc/>
        public string TargetId => TargetInfo.TargetId;

        /// <inheritdoc/>
        public ITarget Opener => TargetInfo.OpenerId != null ?
            ((Browser)Browser).TargetManager.GetAvailableTargets().InnerDictionary.GetValueOrDefault(TargetInfo.OpenerId) : null;

        /// <inheritdoc/>
        public IBrowser Browser => BrowserContext.Browser;

        /// <inheritdoc/>
        IBrowserContext ITarget.BrowserContext => BrowserContext;

        internal BrowserContext BrowserContext { get; }

        internal Task<bool> InitializedTask => InitializedTaskWrapper.Task;

        internal TaskCompletionSource<bool> InitializedTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task CloseTask => CloseTaskWrapper.Task;

        internal TaskCompletionSource<bool> CloseTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task<IPage> PageTask { get; set; }

        internal ITargetManager TargetManager { get; }

        internal bool IsInitialized { get; set; }

        internal CDPSession Session { get; }

        internal TargetInfo TargetInfo { get; set; }

        /// <summary>
        /// Returns the <see cref="IPage"/> associated with the target. If the target is not <c>"page"</c> or <c>"background_page"</c> returns <c>null</c>.
        /// </summary>
        /// <returns>a task that returns a <see cref="IPage"/>.</returns>
        public Task<IPage> PageAsync()
        {
            if (_isPageTargetFunc(TargetInfo) && PageTask == null)
            {
                PageTask = CreatePageAsync();
            }

            return PageTask ?? Task.FromResult<IPage>(null);
        }

        /// <summary>
        /// If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.
        /// </summary>
        /// <returns>A task that returns a <see cref="Worker"/>.</returns>
        public Task<Worker> WorkerAsync()
        {
            if (TargetInfo.Type != TargetType.ServiceWorker && TargetInfo.Type != TargetType.SharedWorker)
            {
                return Task.FromResult<Worker>(null);
            }

            if (_workerTask == null)
            {
                _workerTask = WorkerInternalAsync();
            }

            return _workerTask;
        }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="ICDPSession"/>.</returns>
        public async Task<ICDPSession> CreateCDPSessionAsync() => await _sessionFactory(false).ConfigureAwait(false);

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            TargetInfo = targetInfo;

            if (!IsInitialized && (TargetInfo.Type != TargetType.Page || !string.IsNullOrEmpty(TargetInfo.Url)))
            {
                IsInitialized = true;
                InitializedTaskWrapper.TrySetResult(true);
            }
        }

        private async Task<Worker> WorkerInternalAsync()
        {
            var client = Session ?? await _sessionFactory(false).ConfigureAwait(false);
            return new Worker(
                client,
                TargetInfo.Url,
                (_, _, _) => Task.CompletedTask,
                _ => { });
        }

        private async Task<IPage> CreatePageAsync()
        {
            var session = Session ?? await _sessionFactory(true).ConfigureAwait(false);

            return await Page.CreateAsync(
                session,
                this,
                _ignoreHTTPSErrors,
                _defaultViewport,
                _screenshotTaskQueue).ConfigureAwait(false);
        }
    }
}
