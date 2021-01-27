namespace PuppeteerSharp
{
    internal class ContextPayload
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        public ContextPayloadAuxData AuxData { get; set; }

        public string Name { get; set; }
    }
}
