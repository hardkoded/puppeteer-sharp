using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp;

/// <summary>
/// Device request prompts let you respond to the page requesting for a device
/// through an API like WebBluetooth.
/// </summary>
/// <remarks><see cref="DeviceRequestPrompt"/> instances are returned via the <see cref="IPage.WaitForDevicePromptAsync"/>.</remarks>
/// <example>
/// <code source="../PuppeteerSharp.Tests/DeviceRequestPromptTests/WaitForDevicePromptTests.cs" region="DeviceRequestPromptUsage" lang="csharp"/>
/// </example>
public class DeviceRequestPrompt
{
    private readonly string _id;
    private readonly TimeoutSettings _timeoutSettings;
    private bool _handled;
    private ICDPSession _client;

    internal DeviceRequestPrompt(ICDPSession client, TimeoutSettings timeoutSettings, DeviceAccessDeviceRequestPromptedResponse firstEvent)
    {
        _client = client;
        _timeoutSettings = timeoutSettings;
        _id = firstEvent.Id;

        _client.MessageReceived += OnMessageReceived;

        UpdateDevices(firstEvent);
    }

    // Puppeteer uses a waitForDevicePromises instead of events.
    // I think this is more consistent with the rest of our code.
    internal event EventHandler<DeviceRequestPromptDevice> NewDevice;

    /// <summary>
    /// Current list of selectable devices.
    /// </summary>
    public List<DeviceRequestPromptDevice> Devices { get; } = [];

    /// <summary>
    /// Select a device in the prompt's list.
    /// </summary>
    /// <param name="device">The device to select.</param>
    /// <returns>A task that resolves after the select message is processed by the browser.</returns>
    public Task SelectAsync(DeviceRequestPromptDevice device)
    {
        if (device == null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        if (_client == null)
        {
            throw new PuppeteerException("Cannot select device through a detached session!");
        }

        if (!Devices.Contains(device))
        {
            throw new PuppeteerException($"Cannot select unknown device!");
        }

        if (_handled)
        {
            throw new PuppeteerException("Cannot select DeviceRequestPrompt which is already handled!");
        }

        _handled = true;

        return _client.SendAsync("DeviceAccess.selectPrompt", new DeviceAccessSelectPrompt
        {
            RequestId = _id,
            DeviceId = device.Id,
        });
    }

    /// <summary>
    /// Resolve to the first device in the prompt matching a filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="options">The options.</param>
    /// <returns>A task that resolves to the first device matching the filter.</returns>
    public Task<DeviceRequestPromptDevice> WaitForDeviceAsync(Func<DeviceRequestPromptDevice, bool> filter, WaitForOptions options = default)
    {
        foreach (var device in Devices.Where(filter))
        {
            return Task.FromResult(device);
        }

        var tcs = new TaskCompletionSource<DeviceRequestPromptDevice>();
        var timeout = options?.Timeout ?? _timeoutSettings.Timeout;
        var task = tcs.Task.WithTimeout(timeout);

        NewDevice += OnNewDevice;
        return task;

        void OnNewDevice(object sender, DeviceRequestPromptDevice e)
        {
            if (filter(e))
            {
                tcs.TrySetResult(e);
                NewDevice -= OnNewDevice;
            }
        }
    }

    /// <summary>
    /// Cancel the prompt.
    /// </summary>
    /// <returns>A task that resolves after the cancel message is processed by the browser.</returns>
    public Task CancelAsync()
    {
        if (_client == null)
        {
            throw new PuppeteerException("Cannot cancel prompt through detached session!");
        }

        if (_handled)
        {
            throw new PuppeteerException("Cannot cancel DeviceRequestPrompt which is already handled!");
        }

        _handled = true;

        return _client.SendAsync("DeviceAccess.cancelPrompt", new DeviceAccessCancelPrompt
        {
            RequestId = _id,
        });
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        try
        {
            switch (e.MessageID)
            {
                case "DeviceAccess.deviceRequestPrompted":
                    // This is not upstream. But in upstream they have individual events.
                    if (!_handled)
                    {
                        UpdateDevices(e.MessageData.ToObject<DeviceAccessDeviceRequestPromptedResponse>());
                    }

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

    private void UpdateDevices(DeviceAccessDeviceRequestPromptedResponse e)
    {
        if (e.Id != _id)
        {
            return;
        }

        foreach (var rawDevice in e.Devices)
        {
            if (Devices.Any(d => d.Id == rawDevice.Id))
            {
                continue;
            }

            var newDevice = new DeviceRequestPromptDevice(rawDevice.Name, rawDevice.Id);
            Devices.Add(newDevice);
            NewDevice?.Invoke(this, newDevice);
        }
    }
}
