using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Messaging;

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
    internal DeviceRequestPrompt(CDPSession client, TimeoutSettings timeoutSettings, DeviceRequestPromptedEvent firstEvent)
    {
        Client = client;
        TimeoutSettings = timeoutSettings;
    }

    /// <summary>
    /// Current list of selectable devices.
    /// </summary>
    public IReadOnlyCollection<DeviceRequestPromptDevice> Devices { get; }

    internal CDPSession Client { get; set; }

    internal TimeoutSettings TimeoutSettings { get; set; }

    /// <summary>
    /// Select a device in the prompt's list.
    /// </summary>
    /// <param name="device">The device to select.</param>
    /// <returns>A task that resolves after the select message is processed by the browser.</returns>
    public Task SelectAsync(DeviceRequestPromptDevice device)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Resolve to the first device in the prompt matching a filter.
    /// </summary>
    /// <param name="filter">The filter to apply.</param>
    /// <param name="options">The options.</param>
    /// <returns>A task that resolves to the first device matching the filter.</returns>
    public Task<DeviceRequestPromptDevice> WaitForDeviceAsync(Func<DeviceRequestPromptDevice, bool> filter, WaitTimeoutOptions options = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel the prompt.
    /// </summary>
    /// <returns>A task that resolves after the cancel message is processed by the browser.</returns>
    public Task CancelAsync()
    {
        throw new NotImplementedException();
    }
}
