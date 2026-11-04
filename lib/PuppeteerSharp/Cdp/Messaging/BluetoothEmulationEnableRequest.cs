namespace PuppeteerSharp.Cdp.Messaging;

internal class BluetoothEmulationEnableRequest
{
    public AdapterState State { get; set; }

    public bool LeSupported { get; set; }
}
