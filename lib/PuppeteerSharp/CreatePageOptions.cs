namespace PuppeteerSharp;

/// <summary>
/// Options for creating a new page.
/// </summary>
public class CreatePageOptions
{
    /// <summary>
    /// Gets or sets the type of page to create.
    /// </summary>
    public CreatePageType? Type { get; set; }

    /// <summary>
    /// Gets or sets the window bounds for the new page.
    /// </summary>
    public WindowBounds WindowBounds { get; set; }

    /// <summary>
    /// Gets or sets whether to create the page in background.
    /// </summary>
    public bool? Background { get; set; }
}
