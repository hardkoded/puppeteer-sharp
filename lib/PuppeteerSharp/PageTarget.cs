using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    /// <summary>
    /// Page target.
    /// </summary>
    public class PageTarget : Target
    {
        private readonly bool _ignoreHTTPSErrors;
        private readonly ViewPortOptions _defaultViewport;

        internal PageTarget(TargetInfo targetInfo, CDPSession session, BrowserContext context, ITargetManager targetManager, Func<bool, Task<CDPSession>> sessionFactory, bool ignoreHTTPSErrors, ViewPortOptions defaultViewport, TaskQueue screenshotTaskQueue)
            : base(targetInfo, session, context, targetManager, sessionFactory, screenshotTaskQueue)
        {
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _defaultViewport = defaultViewport;
            PageTask = null;
        }

        private Task<Page> PageTask { get; set; }

        /// <inheritdoc/>
        public override async Task<IPage> PageAsync()
        {
            if (PageTask == null)
            {
                var session = Session ?? await SessionFactory(false).ConfigureAwait(false);

                PageTask = Page.CreateAsync(
                    session,
                    this,
                    _ignoreHTTPSErrors,
                    _defaultViewport,
                    ScreenshotTaskQueue);
            }

            return await PageTask.ConfigureAwait(false);
        }

        internal override void Initialize()
        {
            _ = InitializedTaskWrapper.Task.ContinueWith(
                async initializedTask =>
                {
                    var success = initializedTask.Result;
                    if (success != InitializationStatus.Success)
                    {
                        return;
                    }

                    var opener = Opener as PageTarget;

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
            CheckIfInitialized();
        }

        /// <inheritdoc/>
        protected internal override void CheckIfInitialized()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = !string.IsNullOrEmpty(TargetInfo.Url);
            if (IsInitialized)
            {
                InitializedTaskWrapper.TrySetResult(InitializationStatus.Success);
            }
        }
    }
}
