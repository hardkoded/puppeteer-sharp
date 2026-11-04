namespace PuppeteerSharp.Cdp.Messaging;

internal class BluetoothEmulationSimulatePreconnectedPeripheralRequest
{
    public string Address { get; set; }

    public string Name { get; set; }

    public BluetoothManufacturerData[] ManufacturerData { get; set; }

    public string[] KnownServiceUuids { get; set; }
}
