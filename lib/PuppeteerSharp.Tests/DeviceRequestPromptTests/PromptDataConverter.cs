using Newtonsoft.Json.Linq;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Tests.DeviceRequestPromptTests;

internal static class PromptDataConverter
{
    public static JToken ToJToken(this DeviceAccessDeviceRequestPromptedResponse promptData)
    {
        var jObject = new JObject { { "id", promptData.Id } };
        var devices = new JArray();
        foreach (var device in promptData.Devices)
        {
            var deviceObject = new JObject { { "name", device.Name }, { "id", device.Id } };
            devices.Add(deviceObject);
        }
        jObject.Add("devices", devices);
        return jObject;
    }
}
