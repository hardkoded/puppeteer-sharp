namespace PuppeteerSharp
{
    /// <summary>
    /// Represents a single autofill address field name/value pair.
    /// </summary>
    public class AutofillAddressFieldEntry
    {
        /// <summary>
        /// Gets or sets the field type name.
        /// Use constants from <see cref="AutofillAddressField"/> or a raw CDP field name string.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        public string Value { get; set; }
    }
}
