using System.Text.Json;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Helpers.Json;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

internal static class PromptDataConverter
{
    public static JsonElement ToJsonElement(this DeviceAccessDeviceRequestPromptedResponse promptData)
        => JsonSerializer.SerializeToElement(promptData, JsonHelper.DefaultJsonSerializerSettings.Value);
}
