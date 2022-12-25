using System;
using System.Diagnostics;
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
        private readonly Func<Task<CDPSession>> _sessionFactory;
        private readonly TaskCompletionSource<bool> _initializedTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private Task<Worker> _workerTask;

        internal Target(
            TargetInfo targetInfo,
            Func<Task<CDPSession>> sessionFactory,
            BrowserContext browserContext)
        {
            TargetInfo = targetInfo;
            _sessionFactory = sessionFactory;
            BrowserContext = browserContext;
            PageTask = null;

            _ = _initializedTaskWrapper.Task.ContinueWith(
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

            CloseTaskWrapper = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            IsInitialized = TargetInfo.Type != TargetType.Page || !string.IsNullOrEmpty(TargetInfo.Url);

            if (IsInitialized)
            {
                _initializedTaskWrapper.TrySetResult(true);
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url => TargetInfo.Url;

        /// <summary>
        /// Gets the type. It will be <see cref="PuppeteerSharp.TargetInfo.Type"/>.
        /// Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type => TargetInfo.Type;

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId => TargetInfo.TargetId;

        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        /// <remarks>
        /// Top-level targets return <c>null</c>.
        /// </remarks>
        public ITarget Opener => TargetInfo.OpenerId != null ?
            ((Browser)Browser).TargetsMap.GetValueOrDefault(TargetInfo.OpenerId) : null;

        /// <summary>
        /// Get the browser the target belongs to.
        /// </summary>
        public IBrowser Browser => BrowserContext.Browser;

        /// <summary>
        /// Get the browser context the target belongs to.
        /// </summary>
        public IBrowserContext BrowserContext { get; }

        internal Task<bool> InitializedTask => _initializedTaskWrapper.Task;

        internal Task CloseTask => CloseTaskWrapper.Task;

        internal TaskCompletionSource<bool> CloseTaskWrapper { get; }

        internal Task<IPage> PageTask { get; set; }

        internal bool IsInitialized { get; set; }

        internal TargetInfo TargetInfo { get; set; }

        /// <summary>
        /// Returns the <see cref="IPage"/> associated with the target. If the target is not <c>"page"</c> or <c>"background_page"</c> returns <c>null</c>
        /// </summary>
        /// <returns>a task that returns a <see cref="IPage"/></returns>
        public Task<IPage> PageAsync()
        {
            if ((TargetInfo.Type == TargetType.Page || TargetInfo.Type == TargetType.BackgroundPage) && PageTask == null)
            {
                PageTask = CreatePageAsync();
            }

            return PageTask ?? Task.FromResult<IPage>(null);
        }

        /// <summary>
        /// If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.
        /// </summary>
        /// <returns>A task that returns a <see cref="Worker"/></returns>
        public Task<Worker> WorkerAsync()
        {
            if (TargetInfo.Type != TargetType.ServiceWorker && TargetInfo.Type != TargetType.SharedWorker)
            {
                return null;
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
        /// <returns>A task that returns a <see cref="ICDPSession"/></returns>
        public async Task<ICDPSession> CreateCDPSessionAsync()
        {
            return await ((Browser)Browser).Connection.CreateSessionAsync(TargetInfo).ConfigureAwait(false);
        }

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            var previousUrl = TargetInfo.Url;
            TargetInfo = targetInfo;

            if (!IsInitialized && (TargetInfo.Type != TargetType.Page || !string.IsNullOrEmpty(TargetInfo.Url)))
            {
                IsInitialized = true;
                _initializedTaskWrapper.TrySetResult(true);
                return;
            }

            if (previousUrl != targetInfo.Url)
            {
                ((Browser)Browser).ChangeTarget(this);
            }
        }

        private async Task<Worker> WorkerInternalAsync()
        {
            var client = await _sessionFactory().ConfigureAwait(false);
            return new Worker(
                client,
                TargetInfo.Url,
                (_, _, _) => Task.CompletedTask,
                _ => { });
        }

        private async Task<IPage> CreatePageAsync()
        {
            var session = await _sessionFactory().ConfigureAwait(false);

            var borwser = (Browser)Browser;

            return await Page.CreateAsync(session, this, borwser.IgnoreHTTPSErrors, borwser.DefaultViewport, borwser.ScreenshotTaskQueue).ConfigureAwait(false);
        }
    }
}
