namespace PuppeteerSharp
{
    public class ContextPayload
    {
        public ContextPayload(dynamic context)
        {
            Id = context.id;
            AuxData = context.auxData.ToObject<ContextPayloadAuxData>();
        }

        public int Id { get; internal set; }

        public ContextPayloadAuxData AuxData { get; internal set; }
    }
}