namespace PuppeteerSharp
{
    public class ContextPayload
    {

        public ContextPayload(dynamic context)
        {
            Id = context.id;
        }

        public string Id { get; internal set; }
    }
}