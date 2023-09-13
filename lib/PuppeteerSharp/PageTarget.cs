using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class PageTarget : Target
    {
        private readonly bool _ignoreHTTPSErrors;
        private readonly ViewPortOptions _defaultViewport;
        private readonly TaskQueue _screenshotTaskQueue;

        public PageTarget(TargetInfo targetInfo, CDPSession session, BrowserContext context, ITargetManager targetManager, Func<bool, Task<CDPSession>> sessionFactory, bool ignoreHTTPSErrors, ViewPortOptions defaultViewport, TaskQueue screenshotTaskQueue)
            : base(targetInfo, session, context, targetManager, sessionFactory)
        {
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _defaultViewport = defaultViewport;
            _screenshotTaskQueue = screenshotTaskQueue;
            PageTask = null;
        }

        internal Task<Page> PageTask { get; set; }

        /// <inheritdoc/>
        public override async Task<IPage> PageAsync()
        {
            if (PageTask == null)
            {
                var session = Session ?? await SessionFactory(true).ConfigureAwait(false);

                PageTask = Page.CreateAsync(
                    session,
                    this,
                    _ignoreHTTPSErrors,
                    _defaultViewport,
                    _screenshotTaskQueue);
            }

            return await PageTask.ConfigureAwait(false);
        }

        protected override void Initialize()
        {
            _ = InitializedTaskWrapper.Task.ContinueWith(
                async initializedTask =>
                {
                    var success = initializedTask.Result;
                    if (!success)
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
        }

        protected override void CheckIfInitialized()
        {
            if (!IsInitialized && (TargetInfo.Type != TargetType.Page || !string.IsNullOrEmpty(TargetInfo.Url)))
            {
                IsInitialized = true;
                InitializedTaskWrapper.TrySetResult(true);
            }
        }
    }
}
