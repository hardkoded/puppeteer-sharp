using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    /// <summary>
    /// Target.
    /// </summary>
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target
    {
        #region Private members
        private Browser _browser;
        private TargetInfo _targetInfo;
        private Task<Page> _pageTask;
        #endregion

        internal bool IsInitialized;

        internal Target(Browser browser, TargetInfo targetInfo)
        {
            _browser = browser;
            _targetInfo = targetInfo;

            InitilizedTaskWrapper = new TaskCompletionSource<bool>();
            IsInitialized = _targetInfo.Type != "page" || _targetInfo.Url != string.Empty;

            if (IsInitialized)
            {
                InitilizedTaskWrapper.SetResult(true);
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
        public string Type => _targetInfo.Type == "page" || _targetInfo.Type == "service_worker" ? _targetInfo.Type : "other";
        /// <summary>
        /// Gets the target identifier.
        /// </summary>
        /// <value>The target identifier.</value>
        public string TargetId => _targetInfo.TargetId;
        internal Task<bool> InitializedTask => InitilizedTaskWrapper.Task;
        internal TaskCompletionSource<bool> InitilizedTaskWrapper { get; }
        #endregion

        /// <summary>
        /// Creates a new <see cref="Page"/>. If the target is not <c>"page"</c> returns <c>null</c>
        /// </summary>
        /// <returns>a task that returns a new <see cref="Page"/></returns>
        public async Task<Page> PageAsync()
        {
            if (_targetInfo.Type == "page" && _pageTask == null)
            {
                _pageTask = await _browser.Connection.CreateSessionAsync(_targetInfo.TargetId)
                    .ContinueWith(clientTask
                    => Page.CreateAsync(clientTask.Result, this, _browser.IgnoreHTTPSErrors, _browser.AppMode, _browser.ScreenshotTaskQueue));
            }

            return await (_pageTask ?? Task.FromResult<Page>(null));
        }

        internal void TargetInfoChanged(TargetInfo targetInfo)
        {
            var previousUrl = _targetInfo.Url;
            _targetInfo = targetInfo;

            if (!IsInitialized && (_targetInfo.Type != "page" || _targetInfo.Url != string.Empty))
            {
                IsInitialized = true;
                InitilizedTaskWrapper.SetResult(true);
                return;
            }

            if (previousUrl != targetInfo.Url)
            {
                _browser.ChangeTarget(this);
            }
        }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="CDPSession"/></returns>
        public Task<CDPSession> CreateCDPSessionAsync() => _browser.Connection.CreateSessionAsync(TargetId);
    }
}
