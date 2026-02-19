namespace PuppeteerSharp;

/// <summary>
/// A bluetooth peripheral to be simulated.
/// </summary>
public class PreconnectedPeripheral
{
    /// <summary>
    /// Gets or sets the address of the peripheral.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the name of the peripheral.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer data.
    /// </summary>
    public BluetoothManufacturerData[] ManufacturerData { get; set; }

    /// <summary>
    /// Gets or sets the known service UUIDs.
    /// </summary>
    public string[] KnownServiceUuids { get; set; }
}
