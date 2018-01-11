namespace PuppeteerSharp
{
    public class FrameEventArgs
    {
        private Frame _frame;

        public FrameEventArgs(Frame frame)
        {
            _frame = frame;
        }
    }
}