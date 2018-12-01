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
    public class Target
    {
        #region Private members
        private TargetInfo _targetInfo;
        private readonly string _targetId;
        private readonly Func<TargetInfo, Task<CDPSession>> _sessionFactory;
        private Task<Page> _pageTask;
        #endregion

        internal bool IsInitialized;

        internal Target(
            TargetInfo targetInfo,
            Func<TargetInfo, Task<CDPSession>> sessionFactory,
            BrowserContext browserContext)
        {
            _targetInfo = targetInfo;
            _targetId = targetInfo.TargetId;
            _sessionFactory = sessionFactory;
            BrowserContext = browserContext;
            _pageTask = null;

            InitilizedTaskWrapper = new TaskCompletionSource<bool>();
            CloseTaskWrapper = new TaskCompletionSource<bool>();
            IsInitialized = _targetInfo.Type != TargetType.Page || _targetInfo.Url != string.Empty;

            if (IsInitialized)
            {
                InitilizedTaskWrapper.TrySetResult(true);
            }
        }

        #region Properties
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url => _targetInfo.Url;
        /// <summary>
        /// Gets the type. It will be <see cref="TargetInfo.Type"/> if it's "page" or "service_worker". Otherwise it will be "other"
        /// </summary>
        /// <value>The type.</value>
        public TargetType Type => _targetInfo.Type;

        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId => _targetInfo.TargetId;

        /// <summary>
        /// Get the target that opened this target
        /// </summary>
        /// <remarks>
        /// Top-level targets return <c>null</c>.
        /// </remarks>
        public Target Opener => _targetInfo.OpenerId != null ?
            Browser.TargetsMap.GetValueOrDefault(_targetInfo.OpenerId) : null;

        /// <summary>
        /// Get the browser the target belongs to.
        /// </summary>
        public Browser Browser => BrowserContext.Browser;

        /// <summary>
        /// Get the browser context the target belongs to.
        /// </summary>
        public BrowserContext BrowserContext { get; }

        internal Task<bool> InitializedTask => InitilizedTaskWrapper.Task;
        internal TaskCompletionSource<bool> InitilizedTaskWrapper { get; }
        internal Task CloseTask => CloseTaskWrapper.Task;
        internal TaskCompletionSource<bool> CloseTaskWrapper { get; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Page"/>. If the target is not <c>"page"</c> or <c>"background_page"</c> returns <c>null</c>
        /// </summary>
        /// <returns>a task that returns a new <see cref="Page"/></returns>
        public Task<Page> PageAsync()
        {
            if ((_targetInfo.Type == TargetType.Page || _targetInfo.Type == TargetType.BackgroundPage) && _pageTask == null)
            {
                _pageTask = CreatePageAsync();
            }

            return _pageTask ?? Task.FromResult<Page>(null);
        }

        private async Task<Page> CreatePageAsync()
        {
            var session = await _sessionFactory(_targetInfo).ConfigureAwait(false);
            return await Page.CreateAsync(session, this, Browser.IgnoreHTTPSErrors, Browser.DefaultViewport, Browser.ScreenshotTaskQueue).ConfigureAwait(false);
        }

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            var previousUrl = _targetInfo.Url;
            _targetInfo = targetInfo;

            if (!IsInitialized && (_targetInfo.Type != TargetType.Page || _targetInfo.Url != string.Empty))
            {
                IsInitialized = true;
                InitilizedTaskWrapper.TrySetResult(true);
                return;
            }

            if (previousUrl != targetInfo.Url)
            {
                Browser.ChangeTarget(this);
            }
        }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="CDPSession"/></returns>
        public Task<CDPSession> CreateCDPSessionAsync() => Browser.Connection.CreateSessionAsync(_targetInfo);
    }
}