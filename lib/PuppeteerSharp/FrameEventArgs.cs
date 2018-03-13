namespace PuppeteerSharp
{
    public class FrameEventArgs
    {
        public Frame Frame { get; set; }

        public FrameEventArgs(Frame frame)
        {
            Frame = frame;
        }
    }
}