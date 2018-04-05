namespace PuppeteerSharp
{
    public class ErrorEventArgs
    {
        public string Error { get; }

        public ErrorEventArgs(string error)
        {
            Error = error;
        }
    }
}