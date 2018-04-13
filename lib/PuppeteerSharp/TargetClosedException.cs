namespace PuppeteerSharp
{
    public class TargetClosedException : PuppeteerException
    {
        public TargetClosedException(string message) : base(message)
        {
        }
    }
}