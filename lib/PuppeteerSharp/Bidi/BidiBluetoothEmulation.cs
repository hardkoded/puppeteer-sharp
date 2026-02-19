#if !CDP_ONLY

using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi.Bluetooth;

namespace PuppeteerSharp.Bidi;

internal class BidiBluetoothEmulation : IBluetoothEmulation
{
    private readonly BrowsingContext _browsingContext;

    public BidiBluetoothEmulation(BrowsingContext browsingContext)
    {
        _browsingContext = browsingContext;
    }

    public async Task EmulateAdapterAsync(AdapterState state, bool leSupported = true)
    {
        var bidiState = state switch
        {
            AdapterState.Absent => WebDriverBiDi.Bluetooth.AdapterState.Absent,
            AdapterState.PoweredOff => WebDriverBiDi.Bluetooth.AdapterState.PoweredOff,
            AdapterState.PoweredOn => WebDriverBiDi.Bluetooth.AdapterState.PoweredOn,
            _ => throw new PuppeteerException($"Unknown adapter state: {state}"),
        };

        await _browsingContext.Session.Driver.Bluetooth.SimulateAdapterAsync(
            new SimulateAdapterCommandParameters(_browsingContext.Id, bidiState)).ConfigureAwait(false);
    }

    public async Task DisableEmulationAsync()
    {
        await _browsingContext.Session.Driver.Bluetooth.DisableSimulationAsync(
            new DisableSimulationCommandParameters(_browsingContext.Id)).ConfigureAwait(false);
    }

    public async Task SimulatePreconnectedPeripheralAsync(PreconnectedPeripheral preconnectedPeripheral)
    {
        var parameters = new SimulatePreconnectedPeripheralCommandParameters(
            _browsingContext.Id,
            preconnectedPeripheral.Address,
            preconnectedPeripheral.Name);

        if (preconnectedPeripheral.ManufacturerData != null)
        {
            foreach (var md in preconnectedPeripheral.ManufacturerData)
            {
                parameters.ManufacturerData.Add(new WebDriverBiDi.Bluetooth.BluetoothManufacturerData((uint)md.Key, md.Data));
            }
        }

        if (preconnectedPeripheral.KnownServiceUuids != null)
        {
            foreach (var uuid in preconnectedPeripheral.KnownServiceUuids)
            {
                parameters.KnownServiceUUIDs.Add(uuid);
            }
        }

        await _browsingContext.Session.Driver.Bluetooth.SimulatePreconnectedPeripheralAsync(parameters).ConfigureAwait(false);
    }
}

#endif
