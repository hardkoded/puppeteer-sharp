namespace PuppeteerSharp
{
    /// <summary>
    /// <see cref="IPage.FrameAttached"/>, <see cref="IPage.FrameDetached"/> and <see cref="IPage.FrameNavigated"/> arguments.
    /// </summary>
    public class FrameEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEventArgs"/> class.
        /// </summary>
        /// <param name="frame">Frame.</param>
        public FrameEventArgs(IFrame frame)
        {
            Frame = frame;
        }

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public IFrame Frame { get; set; }
    }
}
