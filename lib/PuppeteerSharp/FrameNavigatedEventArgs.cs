using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp;

/// <summary>
/// <see cref="IPage.FrameNavigated"/> arguments.
/// </summary>
public record FrameNavigatedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FrameNavigatedEventArgs"/> class.
    /// </summary>
    /// <param name="frame">Frame.</param>
    /// <param name="type">Navigation type.</param>
    /// <param name="navigatedWithinDocument">Whether this is a within-document navigation.</param>
    internal FrameNavigatedEventArgs(IFrame frame, NavigationType type, bool navigatedWithinDocument = false)
    {
        Frame = frame;
        Type = type;
        NavigatedWithinDocument = navigatedWithinDocument;
    }

    /// <summary>
    /// Gets or sets the frame.
    /// </summary>
    /// <value>The frame.</value>
    public IFrame Frame { get; set; }

    /// <summary>
    /// Navigation type.
    /// </summary>
    public NavigationType Type { get; }

    /// <summary>
    /// Whether this is a within-document navigation (e.g., via History API).
    /// </summary>
    internal bool NavigatedWithinDocument { get; }
}
