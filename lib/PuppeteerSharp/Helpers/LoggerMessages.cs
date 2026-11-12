using System;
using Microsoft.Extensions.Logging;

namespace PuppeteerSharp.Helpers
{
    /// <summary>
    /// Source-generated logger messages.
    /// </summary>
    internal static partial class LoggerMessages
    {
        [LoggerMessage(Level = LogLevel.Trace, Message = "BiDi mapper debug: {Message}")]
        internal static partial void BidiMapperDebug(this ILogger logger, string message);

        [LoggerMessage(Level = LogLevel.Debug, Message = "BiDi-over-CDP mapper initialized for target {TargetId}")]
        internal static partial void BidiMapperInitialized(this ILogger logger, string targetId);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Protocol error: Connection closed. Most likely the page has been closed.")]
        internal static partial void ConnectionClosedPageClosed(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Warning, Message = "FrameElementAsync: Error disposing iframe")]
        internal static partial void ErrorDisposingIframe(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "Error getting iframe accessibility snapshot")]
        internal static partial void ErrorGettingIframeAccessibilitySnapshot(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "{Message}")]
        internal static partial void ErrorMessage(this ILogger logger, string message);

        [LoggerMessage(Level = LogLevel.Error, Message = "Error while parsing extension id from origin: {Origin}")]
        internal static partial void ErrorParsingExtensionId(this ILogger logger, string origin);

        [LoggerMessage(Level = LogLevel.Error, Message = "Error processing mapper binding call")]
        internal static partial void ErrorProcessingMapperBindingCall(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Error while disconnecting")]
        internal static partial void ErrorWhileDisconnecting(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "{Message}")]
        internal static partial void ErrorWithException(this ILogger logger, Exception exception, string message);

        [LoggerMessage(Level = LogLevel.Error, Message = "ExecutionContext not found from message.")]
        internal static partial void ExecutionContextNotFound(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to activate primary target")]
        internal static partial void FailedToActivatePrimaryTarget(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to adopt handle")]
        internal static partial void FailedToAdoptHandle(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to attach to service worker.")]
        internal static partial void FailedToAttachToServiceWorker(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to block service worker registration")]
        internal static partial void FailedToBlockServiceWorkerRegistration(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to check download {Browser} for {Platform} from {BaseUrl}")]
        internal static partial void FailedToCheckDownload(this ILogger logger, Exception exception, SupportedBrowser browser, Platform platform, string baseUrl);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to close browser")]
        internal static partial void FailedToCloseBrowser(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to close browser context")]
        internal static partial void FailedToCloseBrowserContext(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to complete async handling of SendAsync for {Method}")]
        internal static partial void FailedToCompleteAsyncHandling(this ILogger logger, Exception exception, string method);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to continue with auth")]
        internal static partial void FailedToContinueWithAuth(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to deny permission {Permission}")]
        internal static partial void FailedToDenyPermission(this ILogger logger, Exception exception, OverridePermission permission);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to deserialize response")]
        internal static partial void FailedToDeserializeResponse(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to destroy BiDi handles")]
        internal static partial void FailedToDestroyBidiHandles(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to download from {Url}, falling back to dashboard URL: {FallbackUrl}")]
        internal static partial void FailedToDownloadFallingBack(this ILogger logger, string url, string fallbackUrl);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to fail blocked service worker request")]
        internal static partial void FailedToFailBlockedServiceWorkerRequest(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to FinalizeInterceptionsAsync")]
        internal static partial void FailedToFinalizeInterceptions(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to process message {Message}")]
        internal static partial void FailedToProcessMessage(this ILogger logger, Exception exception, string message);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to process OOPIF message {MessageId}")]
        internal static partial void FailedToProcessOopifMessage(this ILogger logger, Exception exception, string messageId);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to read screencast file.")]
        internal static partial void FailedToReadScreencastFile(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to register speculative session")]
        internal static partial void FailedToRegisterSpeculativeSession(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to replay request interception")]
        internal static partial void FailedToReplayRequestInterception(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to reset permission {Permission}")]
        internal static partial void FailedToResetPermission(this ILogger logger, Exception exception, string permission);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to resolve fallback download URL from dashboard")]
        internal static partial void FailedToResolveFallbackUrl(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to resume service worker.")]
        internal static partial void FailedToResumeServiceWorker(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to run setup.exe")]
        internal static partial void FailedToRunSetupExe(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send BiDi message via CDP")]
        internal static partial void FailedToSendBidiMessageViaCdp(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to send browser close command")]
        internal static partial void FailedToSendBrowserCloseCommand(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to call setAutoAttach and runIfWaitingForDebugger")]
        internal static partial void FailedToSetAutoAttach(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to set viewport")]
        internal static partial void FailedToSetViewport(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to set window bounds")]
        internal static partial void FailedToSetWindowBounds(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to setup network block list")]
        internal static partial void FailedToSetupNetworkBlockList(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to stop driver")]
        internal static partial void FailedToStopDriver(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to stop screen recording.")]
        internal static partial void FailedToStopScreenRecording(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to stop screencast.")]
        internal static partial void FailedToStopScreencast(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to stop screencast: {Error}")]
        internal static partial void FailedToStopScreencastWithError(this ILogger logger, string error);

        [LoggerMessage(Level = LogLevel.Error, Message = "Failed to unmount dmg")]
        internal static partial void FailedToUnmountDmg(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "INTERNAL ERROR: missing context with id = {ContextId}")]
        internal static partial void MissingContext(this ILogger logger, int contextId);

        [LoggerMessage(Level = LogLevel.Error, Message = "DOM.scrollIntoViewIfNeeded is not supported")]
        internal static partial void ScrollIntoViewIfNeededNotSupported(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "silentDetach failed.")]
        internal static partial void SilentDetachFailed(this ILogger logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Warning, Message = "Timed out waiting for confirmation that target {TargetId} was closed. Target.closeTarget was acknowledged but no Target.detachedFromTarget/targetDestroyed event followed within {Timeout}ms; this can happen when the close races an in-flight navigation. Continuing.")]
        internal static partial void TimedOutWaitingForTargetClose(this ILogger logger, string targetId, int timeout);

        [LoggerMessage(Level = LogLevel.Trace, Message = "◀ Receive {Message}")]
        internal static partial void TraceReceive(this ILogger logger, string message);

        [LoggerMessage(Level = LogLevel.Trace, Message = "Send ► {Message}")]
        internal static partial void TraceSend(this ILogger logger, string message);

        [LoggerMessage(Level = LogLevel.Information, Message = "Trying to find download URL via {DashboardUrl}")]
        internal static partial void TryingDashboardUrl(this ILogger logger, string dashboardUrl);

        [LoggerMessage(Level = LogLevel.Warning, Message = "{Message}")]
        internal static partial void WarningMessage(this ILogger logger, string message);
    }
}
