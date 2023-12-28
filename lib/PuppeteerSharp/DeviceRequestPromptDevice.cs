namespace PuppeteerSharp;

/// <summary>
/// Device in a request prompt.
/// </summary>
public record DeviceRequestPromptDevice
{
    /// <summary>
    /// Device name as it appears in a prompt.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Device id during a prompt.
    /// </summary>
    public string Id { get; set; }
}
