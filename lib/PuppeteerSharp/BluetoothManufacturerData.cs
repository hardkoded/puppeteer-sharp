namespace PuppeteerSharp;

/// <summary>
/// Represents the simulated bluetooth peripheral's manufacturer data.
/// </summary>
public class BluetoothManufacturerData
{
    /// <summary>
    /// Gets or sets the company identifier, as defined by the Bluetooth SIG.
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer-specific data as a base64-encoded string.
    /// </summary>
    public string Data { get; set; }
}
