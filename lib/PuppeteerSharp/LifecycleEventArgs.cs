namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// Fired for top level page lifecycle events such as navigation, load, paint, etc.
    /// </summary>
    public class LifecycleEventArgs
    {
        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public Frame Frame { get; private set; }

        /// <summary>
        /// Event Name
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifecycleEventArgs"/> class.
        /// </summary>
        /// <param name="frame">Frame.</param>
        /// <param name="eventName">event Name</param>
        public LifecycleEventArgs(Frame frame, string eventName)
        {
            Frame = frame;
            EventName = eventName;
        }
    }
}
