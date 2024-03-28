using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc/>
public class CdpHttpResponse : Response<CdpHttpRequest>
{
    private static readonly Regex _extraInfoLines = new(@"[^ ]* [^ ]* (?<text>.*)", RegexOptions.Multiline);
    private readonly CDPSession _client;
    private readonly bool _fromDiskCache;
    private byte[] _buffer;

    internal CdpHttpResponse(
        CDPSession client,
        CdpHttpRequest request,
        ResponsePayload responseMessage,
        ResponseReceivedExtraInfoResponse extraInfo)
    {
        _client = client;
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
                Headers[keyValue.Key] = keyValue.Value;
            }
        }

        SecurityDetails = responseMessage.SecurityDetails;
        RemoteAddress = new RemoteAddress
        {
            IP = responseMessage.RemoteIPAddress,
            Port = responseMessage.RemotePort,
        };
    }

    /// <inheritdoc/>
    public override bool FromCache => _fromDiskCache || (Request?.FromMemoryCache ?? false);

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
                var response = await _client.SendAsync<NetworkGetResponseBodyResponse>("Network.getResponseBody", new NetworkGetResponseBodyRequest
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

        var match = _extraInfoLines.Match(firstLine);
        if (!match.Success)
        {
            return null;
        }

        var statusText = match.Groups["text"].Value;
        return statusText;
    }
}
