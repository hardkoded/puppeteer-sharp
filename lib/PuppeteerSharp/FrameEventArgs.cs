namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="Page.FrameAttached"/>, <see cref="Page.FrameDetached"/> and <see cref="Page.FrameNavigated"/> arguments.
    /// </summary>
    public class FrameEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEventArgs"/> class.
        /// </summary>
        /// <param name="frame">Frame.</param>
        public FrameEventArgs(Frame frame)
        {
            Frame = frame;
        }

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public Frame Frame { get; set; }
    }
}
