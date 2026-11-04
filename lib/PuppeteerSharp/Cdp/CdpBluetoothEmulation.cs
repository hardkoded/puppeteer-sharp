using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

internal class CdpBluetoothEmulation : IBluetoothEmulation
{
    private readonly Connection _connection;

    public CdpBluetoothEmulation(Connection connection)
    {
        _connection = connection;
    }

    public async Task EmulateAdapterAsync(AdapterState state, bool leSupported = true)
    {
        // Bluetooth spec requires overriding the existing adapter (step 6). From the CDP
        // perspective, it means disabling the emulation first.
        // https://webbluetoothcg.github.io/web-bluetooth/#bluetooth-simulateAdapter-command
        await _connection.SendAsync("BluetoothEmulation.disable").ConfigureAwait(false);
        await _connection.SendAsync("BluetoothEmulation.enable", new BluetoothEmulationEnableRequest
        {
            State = state,
            LeSupported = leSupported,
        }).ConfigureAwait(false);
    }

    public async Task DisableEmulationAsync()
    {
        await _connection.SendAsync("BluetoothEmulation.disable").ConfigureAwait(false);
    }

    public async Task SimulatePreconnectedPeripheralAsync(PreconnectedPeripheral preconnectedPeripheral)
    {
        await _connection.SendAsync(
            "BluetoothEmulation.simulatePreconnectedPeripheral",
            new BluetoothEmulationSimulatePreconnectedPeripheralRequest
            {
                Address = preconnectedPeripheral.Address,
                Name = preconnectedPeripheral.Name,
                ManufacturerData = preconnectedPeripheral.ManufacturerData,
                KnownServiceUuids = preconnectedPeripheral.KnownServiceUuids,
            }).ConfigureAwait(false);
    }
}
