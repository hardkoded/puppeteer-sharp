namespace PuppeteerSharp
{
    internal class ContextPayload
    {
        internal ContextPayload(dynamic context)
        {
            Id = context.id;
            AuxData = context.auxData.ToObject<ContextPayloadAuxData>();
        }

        internal int Id { get; }
        internal ContextPayloadAuxData AuxData { get; }
    }
}