#if NET8_0_OR_GREATER
// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using PuppeteerSharp.BrowserData;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Helpers.Json;

/// <summary>
/// We need this class for AOT.
/// </summary>
[JsonSourceGenerationOptions]
[JsonSerializable(typeof(AccessibilityGetFullAXTreeResponse))]
[JsonSerializable(typeof(AccessibilityQueryAXTreeRequest))]
[JsonSerializable(typeof(AccessibilityQueryAXTreeResponse))]
[JsonSerializable(typeof(BasicFrameResponse))]
[JsonSerializable(typeof(BindingCalledResponse))]
[JsonSerializable(typeof(BrowserGetVersionResponse))]
[JsonSerializable(typeof(BrowserGrantPermissionsRequest))]
[JsonSerializable(typeof(BrowserResetPermissionsRequest))]
[JsonSerializable(typeof(BoundingBox[]))]
[JsonSerializable(typeof(BoundingBox))]
[JsonSerializable(typeof(CertificateErrorResponse))]
[JsonSerializable(typeof(ConnectionError))]
[JsonSerializable(typeof(ConnectionRequest))]
[JsonSerializable(typeof(ConnectionResponse))]
[JsonSerializable(typeof(ConnectionResponseParams))]
[JsonSerializable(typeof(ContinueWithAuthRequest))]
[JsonSerializable(typeof(ContinueWithAuthRequestChallengeResponse))]
[JsonSerializable(typeof(CreateBrowserContextResponse))]
[JsonSerializable(typeof(CssGetStyleSheetTextRequest))]
[JsonSerializable(typeof(CssGetStyleSheetTextResponse))]
[JsonSerializable(typeof(CSSStopRuleUsageTrackingResponse))]
[JsonSerializable(typeof(CSSStyleSheetAddedResponse))]
[JsonSerializable(typeof(DebuggerGetScriptSourceRequest))]
[JsonSerializable(typeof(DebuggerGetScriptSourceResponse))]
[JsonSerializable(typeof(DebuggerScriptParsedResponse))]
[JsonSerializable(typeof(DebuggerSetSkipAllPausesRequest))]
[JsonSerializable(typeof(DeviceAccessCancelPrompt))]
[JsonSerializable(typeof(DeviceAccessDeviceRequestPromptedResponse))]
[JsonSerializable(typeof(DeviceAccessSelectPrompt))]
[JsonSerializable(typeof(Dictionary<string, int>))]
[JsonSerializable(typeof(DispatchKeyEventType))]
[JsonSerializable(typeof(DomDescribeNodeRequest))]
[JsonSerializable(typeof(DomDescribeNodeResponse))]
[JsonSerializable(typeof(DomGetBoxModelRequest))]
[JsonSerializable(typeof(DomGetBoxModelResponse))]
[JsonSerializable(typeof(DomGetContentQuadsRequest))]
[JsonSerializable(typeof(DomGetFrameOwnerRequest))]
[JsonSerializable(typeof(DomGetFrameOwnerResponse))]
[JsonSerializable(typeof(DomResolveNodeRequest))]
[JsonSerializable(typeof(DomResolveNodeResponse))]
[JsonSerializable(typeof(DomScrollIntoViewIfNeededRequest))]
[JsonSerializable(typeof(DomSetFileInputFilesRequest))]
[JsonSerializable(typeof(DragEventType))]
[JsonSerializable(typeof(EmulateTimezoneRequest))]
[JsonSerializable(typeof(EmulationSetCPUThrottlingRateRequest))]
[JsonSerializable(typeof(EmulationSetDefaultBackgroundColorOverrideColor))]
[JsonSerializable(typeof(EmulationSetDefaultBackgroundColorOverrideRequest))]
[JsonSerializable(typeof(EmulationSetDeviceMetricsOverrideRequest))]
[JsonSerializable(typeof(EmulationSetEmulatedMediaFeatureRequest))]
[JsonSerializable(typeof(EmulationSetEmulatedMediaTypeRequest))]
[JsonSerializable(typeof(EmulationSetEmulatedVisionDeficiencyRequest))]
[JsonSerializable(typeof(EmulationSetIdleOverrideRequest))]
[JsonSerializable(typeof(EmulationSetScriptExecutionDisabledRequest))]
[JsonSerializable(typeof(EmulationSetTouchEmulationEnabledRequest))]
[JsonSerializable(typeof(EvaluateExceptionResponseDetails))]
[JsonSerializable(typeof(EvaluateExceptionResponseInfo))]
[JsonSerializable(typeof(EvaluateExceptionResponseStackTrace))]
[JsonSerializable(typeof(EvaluateHandleResponse))]
[JsonSerializable(typeof(EvaluationExceptionResponseCallFrame))]
[JsonSerializable(typeof(FetchAuthRequiredResponse))]
[JsonSerializable(typeof(FetchContinueRequestRequest))]
[JsonSerializable(typeof(FetchEnableRequest))]
[JsonSerializable(typeof(FetchFailRequest))]
[JsonSerializable(typeof(FetchFulfillRequest))]
[JsonSerializable(typeof(FetchRequestPausedResponse))]
[JsonSerializable(typeof(FileChooserAction))]
[JsonSerializable(typeof(GetBrowserContextsResponse))]
[JsonSerializable(typeof(GetContentQuadsResponse))]
[JsonSerializable(typeof(ChromeGoodVersionsResult))]
[JsonSerializable(typeof(Header))]
[JsonSerializable(typeof(InputDispatchDragEventRequest))]
[JsonSerializable(typeof(InputDispatchKeyEventRequest))]
[JsonSerializable(typeof(InputDispatchMouseEventRequest))]
[JsonSerializable(typeof(InputDispatchTouchEventRequest))]
[JsonSerializable(typeof(InputInsertTextRequest))]
[JsonSerializable(typeof(InputSetInterceptDragsRequest))]
[JsonSerializable(typeof(IOCloseRequest))]
[JsonSerializable(typeof(IOReadRequest))]
[JsonSerializable(typeof(IOReadResponse))]
[JsonSerializable(typeof(LifecycleEventResponse))]
[JsonSerializable(typeof(LoadingFailedEventResponse))]
[JsonSerializable(typeof(LoadingFinishedEventResponse))]
[JsonSerializable(typeof(LogEntryAddedResponse))]
[JsonSerializable(typeof(MouseEventType))]
[JsonSerializable(typeof(NavigatedWithinDocumentResponse))]
[JsonSerializable(typeof(NavigationType))]
[JsonSerializable(typeof(NetworkEmulateNetworkConditionsRequest))]
[JsonSerializable(typeof(NetworkGetCookiesRequest))]
[JsonSerializable(typeof(NetworkGetCookiesResponse))]
[JsonSerializable(typeof(NetworkGetResponseBodyRequest))]
[JsonSerializable(typeof(NetworkGetResponseBodyResponse))]
[JsonSerializable(typeof(NetworkSetCacheDisabledRequest))]
[JsonSerializable(typeof(NetworkSetCookiesRequest))]
[JsonSerializable(typeof(NetworkSetExtraHTTPHeadersRequest))]
[JsonSerializable(typeof(NetworkSetUserAgentOverrideRequest))]
[JsonSerializable(typeof(PageAddScriptToEvaluateOnNewDocumentRequest))]
[JsonSerializable(typeof(PageAddScriptToEvaluateOnNewDocumentResponse))]
[JsonSerializable(typeof(PageCaptureScreenshotRequest))]
[JsonSerializable(typeof(PageCaptureScreenshotResponse))]
[JsonSerializable(typeof(PageConsoleResponse))]
[JsonSerializable(typeof(PageCreateIsolatedWorldRequest))]
[JsonSerializable(typeof(PageFileChooserOpenedResponse))]
[JsonSerializable(typeof(PageFrameAttachedResponse))]
[JsonSerializable(typeof(PageFrameDetachedResponse))]
[JsonSerializable(typeof(PageFrameNavigatedResponse))]
[JsonSerializable(typeof(PageGetFrameTree))]
[JsonSerializable(typeof(PageGetFrameTreeResponse))]
[JsonSerializable(typeof(PageGetLayoutMetricsResponse))]
[JsonSerializable(typeof(PageGetNavigationHistoryResponse))]
[JsonSerializable(typeof(PageHandleFileChooserRequest))]
[JsonSerializable(typeof(PageHandleJavaScriptDialogRequest))]
[JsonSerializable(typeof(PageJavascriptDialogOpeningResponse))]
[JsonSerializable(typeof(PageNavigateRequest))]
[JsonSerializable(typeof(PageNavigateResponse))]
[JsonSerializable(typeof(PageNavigateToHistoryEntryRequest))]
[JsonSerializable(typeof(PagePrintToPDFRequest))]
[JsonSerializable(typeof(PagePrintToPDFResponse))]
[JsonSerializable(typeof(PageReloadRequest))]
[JsonSerializable(typeof(PageRemoveScriptToEvaluateOnNewDocumentRequest))]
[JsonSerializable(typeof(PageSetBypassCSPRequest))]
[JsonSerializable(typeof(PageSetInterceptFileChooserDialog))]
[JsonSerializable(typeof(PageSetLifecycleEventsEnabledRequest))]
[JsonSerializable(typeof(PerformanceGetMetricsResponse))]
[JsonSerializable(typeof(PerformanceMetricsResponse))]
[JsonSerializable(typeof(ProfilerStartPreciseCoverageRequest))]
[JsonSerializable(typeof(ProfilerTakePreciseCoverageResponse))]
[JsonSerializable(typeof(Point))]
[JsonSerializable(typeof(RemoteObject))]
[JsonSerializable(typeof(RemoteObjectSubtype))]
[JsonSerializable(typeof(RemoteObjectType))]
[JsonSerializable(typeof(RequestServedFromCacheResponse))]
[JsonSerializable(typeof(RequestWillBeSentResponse))]
[JsonSerializable(typeof(ResponsePayload))]
[JsonSerializable(typeof(ResponseReceivedExtraInfoResponse))]
[JsonSerializable(typeof(ResponseReceivedResponse))]
[JsonSerializable(typeof(RuntimeAddBindingRequest))]
[JsonSerializable(typeof(RuntimeCallFunctionOnRequest))]
[JsonSerializable(typeof(RuntimeCallFunctionOnRequestArgument))]
[JsonSerializable(typeof(RuntimeCallFunctionOnRequestArgumentValue))]
[JsonSerializable(typeof(RuntimeCallFunctionOnResponse))]
[JsonSerializable(typeof(RuntimeEvaluateRequest))]
[JsonSerializable(typeof(RuntimeExceptionThrownResponse))]
[JsonSerializable(typeof(RuntimeExecutionContextCreatedResponse))]
[JsonSerializable(typeof(RuntimeExecutionContextDestroyedResponse))]
[JsonSerializable(typeof(RuntimeGetPropertiesRequest))]
[JsonSerializable(typeof(RuntimeGetPropertiesResponse))]
[JsonSerializable(typeof(RuntimeQueryObjectsRequest))]
[JsonSerializable(typeof(RuntimeQueryObjectsResponse))]
[JsonSerializable(typeof(RuntimeReleaseObjectRequest))]
[JsonSerializable(typeof(RuntimeRemoveBindingRequest))]
[JsonSerializable(typeof(SecurityHandleCertificateErrorResponse))]
[JsonSerializable(typeof(SecuritySetIgnoreCertificateErrorsRequest))]
[JsonSerializable(typeof(SecuritySetOverrideCertificateErrorsRequest))]
[JsonSerializable(typeof(StackTrace))]
[JsonSerializable(typeof(StackTraceId))]
[JsonSerializable(typeof(TargetActivateTargetRequest))]
[JsonSerializable(typeof(TargetAttachedToTargetResponse))]
[JsonSerializable(typeof(TargetAttachToTargetRequest))]
[JsonSerializable(typeof(TargetAttachToTargetResponse))]
[JsonSerializable(typeof(TargetCloseTargetRequest))]
[JsonSerializable(typeof(TargetCreateBrowserContextRequest))]
[JsonSerializable(typeof(TargetCreatedResponse))]
[JsonSerializable(typeof(TargetCreateTargetRequest))]
[JsonSerializable(typeof(TargetCreateTargetResponse))]
[JsonSerializable(typeof(TargetDestroyedResponse))]
[JsonSerializable(typeof(TargetDetachedFromTargetResponse))]
[JsonSerializable(typeof(TargetDetachFromTargetRequest))]
[JsonSerializable(typeof(TargetDisposeBrowserContextRequest))]
[JsonSerializable(typeof(TargetSendMessageToTargetRequest))]
[JsonSerializable(typeof(TargetSetAutoAttachRequest))]
[JsonSerializable(typeof(TargetSetDiscoverTargetsRequest))]
[JsonSerializable(typeof(TargetType))]
[JsonSerializable(typeof(TracingCompleteResponse))]
[JsonSerializable(typeof(TracingStartRequest))]
[JsonSerializable(typeof(WSEndpointResponse))]
[JsonSerializable(typeof(TargetInfo))]
[JsonSerializable(typeof(WaitForFunctionPollingOption))]
[JsonSerializable(typeof(WaitForOptions))]
[JsonSerializable(typeof(GeolocationOption))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(List<object>))]

// Primitive list
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(ushort))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(char))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTimeOffset))]
[JsonSerializable(typeof(TimeSpan))]
[JsonSerializable(typeof(JsonArray))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(JsonDocument))]
#if NET7_0_OR_GREATER
[JsonSerializable(typeof(DateOnly))]
[JsonSerializable(typeof(TimeOnly))]
#endif
#if NET8_0_OR_GREATER
[JsonSerializable(typeof(Half))]
[JsonSerializable(typeof(Int128))]
[JsonSerializable(typeof(UInt128))]
#endif
internal partial class SystemTextJsonSerializationContext : JsonSerializerContext
{
}
#endif
