using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    [DebuggerDisplay("Target {Type} - {Url}")]
    public class Target : IEquatable<Target>
    {
        #region Private members
        private Browser _browser;
        private TargetInfo _targetInfo;
        private Task<Page> _pageTask;
        #endregion

        internal bool IsInitialized;

        public Target(Browser browser, TargetInfo targetInfo)
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
        public string Url => _targetInfo.Url;
        public string Type => _targetInfo.Type == "page" || _targetInfo.Type == "service_worker" ? _targetInfo.Type : "other";
        public Task<bool> InitializedTask => InitilizedTaskWrapper.Task;
        public TaskCompletionSource<bool> InitilizedTaskWrapper { get; }
        public string TargetId => _targetInfo.TargetId;
        #endregion

        public async Task<Page> Page()
        {
            if (_targetInfo.Type == "page" && _pageTask == null)
            {
                _pageTask = await _browser.Connection.CreateSession(_targetInfo.TargetId)
                    .ContinueWith(clientTask
                    => PuppeteerSharp.Page.CreateAsync(clientTask.Result, this, _browser.IgnoreHTTPSErrors, _browser.AppMode, _browser.ScreenshotTaskQueue));
                var client = await _browser.Connection.CreateSession(_targetInfo.TargetId);
            }

            return await (_pageTask ?? Task.FromResult<Page>(null));
        }

        public void TargetInfoChanged(TargetInfo targetInfo)
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

        public bool Equals(Target other) => TargetId.Equals(other.TargetId);
    }
}
