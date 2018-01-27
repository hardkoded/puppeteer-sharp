namespace PuppeteerSharp
{
    public class FramePayload
    {
        public string Id { get; internal set; }
        public string ParentId { get; internal set; }
        public string Name { get; internal set; }
        public string Url { get; internal set; }
    }
}