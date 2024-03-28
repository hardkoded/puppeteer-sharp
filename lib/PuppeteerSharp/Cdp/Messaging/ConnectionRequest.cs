namespace PuppeteerSharp.Cdp.Messaging
{
    internal class ConnectionRequest
    {
        public int Id { get; set; }

        public string Method { get; set; }

        public object Params { get; set; }

        public string SessionId { get; set; }
    }
}
