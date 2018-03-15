namespace PuppeteerSharp
{
    public class ContextPayload
    {

        public ContextPayload(dynamic context)
        {
            Id = context.id;
            AuxData = context.auxData.ToObject<AuxData>();
        }

        public int Id { get; internal set; }

        public AuxData AuxData { get; internal set; }
    }

    public class AuxData
    {
        public string FrameId { get; set; }
        public bool IsDefault { get; set; }
    }
}