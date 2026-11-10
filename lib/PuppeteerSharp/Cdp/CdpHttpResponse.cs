using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public partial class CdpHttpResponse : Response<CdpHttpRequest>
{
#if NETSTANDARD2_0
    private static readonly Regex _extraInfoLines = new(@"[^ ]* [^ ]* (?<text>.*)", RegexOptions.Multiline);
#endif
    private readonly bool _fromDiskCache;
    private byte[] _buffer;
    private long _encodedDataLength;

    internal CdpHttpResponse(
        CdpHttpRequest request,
        ResponsePayload responseMessage,
        ResponseReceivedExtraInfoResponse extraInfo)
    {
        Request = request;
        Status = extraInfo != null ? extraInfo.StatusCode : responseMessage.Status;
        StatusText = ParseStatusTextFromExtraInfo(extraInfo) ?? responseMessage.StatusText;
        Url = request.Url;
        _fromDiskCache = responseMessage.FromDiskCache;
        FromServiceWorker = responseMessage.FromServiceWorker;

        Headers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        var headers = extraInfo != null ? extraInfo.Headers : responseMessage.Headers;
        if (headers != null)
        {
            foreach (var keyValue in headers)
            {
                var headerName = keyValue.Key.ToLowerInvariant();

                // See https://www.rfc-editor.org/rfc/rfc9110.html#name-field-order for
                // the set-cookie exception.
                Headers[headerName] = HttpUtils.NormalizeHeaderValue(headerName, keyValue.Value);
            }
        }

        SecurityDetails = responseMessage.SecurityDetails;
        RemoteAddress = new RemoteAddress
        {
            IP = responseMessage.RemoteIPAddress,
            Port = responseMessage.RemotePort,
        };
        Timing = responseMessage.Timing;
        SetEncodedDataLength(responseMessage.EncodedDataLength);
    }

    /// <inheritdoc/>
    public override bool FromCache => _fromDiskCache || (Request?.FromMemoryCache ?? false);

    /// <inheritdoc/>
    public override long EncodedDataLength => _encodedDataLength;

    internal TaskCompletionSource<bool> BodyLoadedTaskWrapper { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Returns a Task which resolves to a buffer with response body.
    /// </summary>
    /// <returns>A Task which resolves to a buffer with response body.</returns>
    public override async ValueTask<byte[]> BufferAsync()
    {
        if (_buffer == null)
        {
            await BodyLoadedTaskWrapper.Task.ConfigureAwait(false);

            try
            {
                // Use CDPSession from corresponding request to retrieve body, as its client
                // might have been updated (e.g. for an adopted OOPIF).
                var response = await Request.Client.SendAsync<NetworkGetResponseBodyResponse>("Network.getResponseBody", new NetworkGetResponseBodyRequest
                {
                    RequestId = Request.Id,
                }).ConfigureAwait(false);

                _buffer = response.Base64Encoded
                    ? Convert.FromBase64String(response.Body)
                    : Encoding.UTF8.GetBytes(response.Body);
            }
            catch (Exception ex)
            {
                throw new BufferException("Unable to get response body", ex);
            }
        }

        return _buffer;
    }

    internal void SetEncodedDataLength(long encodedDataLength)
    {
        // Chromium reports -1 when no data was transferred over the network (e.g. data: or file: URLs) or when the
        // size is unknown. Keep the last known value in that case, defaulting to 0.
        if (encodedDataLength >= 0)
        {
            _encodedDataLength = encodedDataLength;
        }
    }

#if NET8_0_OR_GREATER
    [GeneratedRegex(@"[^ ]* [^ ]* (?<text>.*)", RegexOptions.Multiline)]
    private static partial Regex GetExtraInfoLines();
#else
    private static Regex GetExtraInfoLines() => _extraInfoLines;
#endif

    private string ParseStatusTextFromExtraInfo(ResponseReceivedExtraInfoResponse extraInfo)
    {
        if (extraInfo == null || extraInfo.HeadersText == null)
        {
            return null;
        }

        var lines = extraInfo.HeadersText.Split('\r');
        if (lines.Length == 0)
        {
            return null;
        }

        var firstLine = lines[0];

        var match = GetExtraInfoLines().Match(firstLine);
        if (!match.Success)
        {
            return null;
        }

        var statusText = match.Groups["text"].Value;
        return statusText;
    }
}
