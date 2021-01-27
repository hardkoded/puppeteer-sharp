namespace PuppeteerSharp
{
    internal class ContextPayload
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the context payload aux data.
        /// </summary>
        public ContextPayloadAuxData AuxData { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
