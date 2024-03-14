namespace PuppeteerSharp;

/// <summary>
/// Command options. See <see cref="ICDPConnection.SendAsync{T}(string, object, CommandOptions)"/>.
/// </summary>
public class CommandOptions
{
    /// <summary>
    /// Gets or sets the timeout.
    /// </summary>
    /// <value>The timeout.</value>
    public int Timeout { get; set; }
}
