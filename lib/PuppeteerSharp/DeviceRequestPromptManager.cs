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
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp;

/// <summary>
/// Prompt manager.
/// </summary>
public class DeviceRequestPromptManager
{
    private readonly TimeoutSettings _timeoutSettings;
    private ICDPSession _client;
    private TaskCompletionSource<DeviceRequestPrompt> _deviceRequestPromptTcs;

    internal DeviceRequestPromptManager(ICDPSession client, TimeoutSettings timeoutSettings)
    {
        _client = client;
        _timeoutSettings = timeoutSettings;
        _client.MessageReceived += OnMessageReceived;
    }

    internal async Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = default)
    {
        if (_client == null)
        {
            throw new PuppeteerException("Cannot wait for device prompt through detached session!");
        }

        var needsEnable = _deviceRequestPromptTcs == null || _deviceRequestPromptTcs.Task.IsCompleted;
        var enableTask = Task.CompletedTask;

        if (needsEnable)
        {
            _deviceRequestPromptTcs = new TaskCompletionSource<DeviceRequestPrompt>();
            enableTask = _client.SendAsync("DeviceAccess.enable");
        }

        var timeout = options?.Timeout ?? _timeoutSettings.Timeout;
        var task = _deviceRequestPromptTcs.Task.WithTimeout(timeout);

        await Task.WhenAll(enableTask, task).ConfigureAwait(false);

        return await _deviceRequestPromptTcs.Task.ConfigureAwait(false);
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            switch (e.MessageID)
            {
                case "DeviceAccess.deviceRequestPrompted":
                    OnDeviceRequestPrompted(e.MessageData.ToObject<DeviceAccessDeviceRequestPromptedResponse>());
                    break;
                case "Target.detachedFromTarget":
                    _client = null;
                    break;
            }
        }
        catch (Exception ex)
        {
            var message = $"Connection failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
            (_client as CDPSession)?.Close(message);
        }
    }

    private void OnDeviceRequestPrompted(DeviceAccessDeviceRequestPromptedResponse e)
    {
        if (_deviceRequestPromptTcs == null)
        {
            return;
        }

        if (_client == null)
        {
            _deviceRequestPromptTcs.TrySetException(new PuppeteerException("Session closed. Most likely the target has been closed."));
            return;
        }

        var devicePrompt = new DeviceRequestPrompt(_client, _timeoutSettings, e);
        _deviceRequestPromptTcs.TrySetResult(devicePrompt);
    }
}
