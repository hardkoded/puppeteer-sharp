// * MIT License
//  *
//  * Copyright (c) Microsoft Corporation.
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
using System.Net.Http;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp;

internal class DeviceRequestPromptManager
{
    private TaskCompletionSource<DeviceRequestPrompt> _deviceRequestPromptTcs;

    internal DeviceRequestPromptManager(CDPSession client, TimeoutSettings timeoutSettings)
    {
        Client = client;
        TimeoutSettings = timeoutSettings;
        Client.MessageReceived += OnMessageReceived;
    }

    public CDPSession Client { get; set; }

    public TimeoutSettings TimeoutSettings { get; set; }

    public async Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitTimeoutOptions options = default)
    {
        if (Client == null)
        {
            throw new PuppeteerException("Cannot wait for device prompt through detached session!");
        }

        var needsEnable = _deviceRequestPromptTcs == null || _deviceRequestPromptTcs.Task.IsCompleted;
        var enableTask = Task.CompletedTask;

        if (needsEnable)
        {
            _deviceRequestPromptTcs = new TaskCompletionSource<DeviceRequestPrompt>();
            enableTask = Client.SendAsync("DeviceAccess.enable");
        }

        var timeout = options?.Timeout ?? TimeoutSettings.Timeout;
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
                    Client = null;
                    break;
            }
        }
        catch (Exception ex)
        {
            var message = $"Connection failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
            Client?.Close(message);
        }
    }

    private void OnDeviceRequestPrompted(DeviceAccessDeviceRequestPromptedResponse e)
    {
        if (_deviceRequestPromptTcs == null)
        {
            return;
        }

        if (Client == null)
        {
            _deviceRequestPromptTcs.TrySetException(new PuppeteerException("Session closed. Most likely the target has been closed."));
            return;
        }

        var devicePrompt = new DeviceRequestPrompt(Client, TimeoutSettings, e);
        _deviceRequestPromptTcs.TrySetResult(devicePrompt);
    }
}
