using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BluetoothEmulationTests
{
    public class BluetoothEmulationTests : PuppeteerPageBaseTest
    {
        public BluetoothEmulationTests() : base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.AcceptInsecureCerts = true;
            DefaultOptions.Args = [.. DefaultOptions.Args ?? [], "--enable-features=WebBluetoothNewPermissionsBackend,WebBluetooth"];
        }

        [Test, PuppeteerTest("bluetooth-emulation.spec", "request prompt for emulated bluetooth device", "can be canceled")]
        public async Task CanBeCanceled()
        {
            await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            await Page.Bluetooth.EmulateAdapterAsync(AdapterState.PoweredOn);
            await Page.Bluetooth.SimulatePreconnectedPeripheralAsync(new PreconnectedPeripheral
            {
                Address = "09:09:09:09:09:09",
                Name = "SOME_NAME",
                ManufacturerData = [new BluetoothManufacturerData { Key = 17, Data = "AP8BAX8=" }],
                KnownServiceUuids = ["12345678-1234-5678-9abc-def123456789"],
            });

            var devicePromptTask = Page.WaitForDevicePromptAsync();

            var navigatorRequestDeviceTask = Page.EvaluateFunctionAsync<string>(@"async () => {
                const device = await navigator.bluetooth.requestDevice({
                    acceptAllDevices: true,
                    optionalServices: [],
                });
                return device.name;
            }");

            var devicePrompt = await devicePromptTask;
            await devicePrompt.CancelAsync();

            Assert.ThrowsAsync<EvaluationFailedException>(async () => await navigatorRequestDeviceTask);
        }

        [Test, PuppeteerTest("bluetooth-emulation.spec", "request prompt for emulated bluetooth device", "can be selected")]
        public async Task CanBeSelected()
        {
            await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            await Page.Bluetooth.EmulateAdapterAsync(AdapterState.PoweredOn);
            await Page.Bluetooth.SimulatePreconnectedPeripheralAsync(new PreconnectedPeripheral
            {
                Address = "09:09:09:09:09:09",
                Name = "SOME_NAME",
                ManufacturerData = [new BluetoothManufacturerData { Key = 17, Data = "AP8BAX8=" }],
                KnownServiceUuids = ["12345678-1234-5678-9abc-def123456789"],
            });

            var devicePromptTask = Page.WaitForDevicePromptAsync();

            var navigatorRequestDeviceTask = Page.EvaluateFunctionAsync<string>(@"async () => {
                const device = await navigator.bluetooth.requestDevice({
                    acceptAllDevices: true,
                    optionalServices: [],
                });
                return device.name;
            }");

            var devicePrompt = await devicePromptTask;
            await devicePrompt.SelectAsync(devicePrompt.Devices[0]);

            var result = await navigatorRequestDeviceTask;
            Assert.That(result, Is.EqualTo("SOME_NAME"));
        }
    }
}
