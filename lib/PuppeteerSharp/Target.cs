using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target
    {
        #region Private members
        private Browser _browser;
        private TargetInfo _targetInfo;
        private bool _isInitialized;
        #endregion

        public Target(Browser browser, TargetInfo targetInfo)
        {
            _browser = browser;
            _targetInfo = targetInfo;

            InitilizedTaskWrapper = new TaskCompletionSource<bool>();
            _isInitialized = _targetInfo.Type != "page" || _targetInfo.Url != string.Empty;

            if (_isInitialized)
            {
                InitilizedTaskWrapper.SetResult(true);
            }
        }

        #region Properties
        public string Url => _targetInfo.Url;
        public string Type => _targetInfo.Type == "page" || _targetInfo.Type == "service_worker" ? _targetInfo.Type : "other";
        public Task<bool> InitializedTask => InitilizedTaskWrapper.Task;
        public TaskCompletionSource<bool> InitilizedTaskWrapper { get; }
        public string TargetId => _targetInfo.TargetId;
        #endregion

        public async Task<Page> Page()
        {
            if (_targetInfo.Type == "page")
            {
                var client = await _browser.Connection.CreateSession(_targetInfo.TargetId);
                return await PuppeteerSharp.Page.CreateAsync(client, this, _browser.IgnoreHTTPSErrors, _browser.AppMode, _browser.ScreenshotTaskQueue);
            }

            return null;
        }

        public void TargetInfoChanged(TargetInfo targetInfo)
        {
            var previousUrl = _targetInfo.Url;
            _targetInfo = targetInfo;

            if (!_isInitialized && (_targetInfo.Type != "page" || _targetInfo.Url != string.Empty))
            {
                _isInitialized = true;
                InitilizedTaskWrapper.SetResult(true);
            }

            if (previousUrl != targetInfo.Url)
            {
                _browser.ChangeTarget(targetInfo);
            }
        }

        /// <summary>
        /// Creates a Chrome Devtools Protocol session attached to the target.
        /// </summary>
        /// <returns>A task that returns a <see cref="Session"/></returns>
        public Task<Session> CreateCDPSession() => _browser.Connection.CreateSession(TargetId);

    }
}
