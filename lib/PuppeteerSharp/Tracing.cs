namespace PuppeteerSharp
{
    internal class Tracing
    {
        private Session client;

        public Tracing(Session client)
        {
            this.client = client;
        }
    }
}