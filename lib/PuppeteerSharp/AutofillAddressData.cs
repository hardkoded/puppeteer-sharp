using System.Collections.Generic;

namespace PuppeteerSharp
{
    /// <summary>
    /// Address data for autofilling.
    /// See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address.
    /// </summary>
    public class AutofillAddressData
    {
        /// <summary>
        /// Gets or sets the address fields to fill.
        /// Each entry has a <c>Name</c> (see <see cref="AutofillAddressField"/>) and a <c>Value</c>.
        /// </summary>
        public List<AutofillAddressFieldEntry> Fields { get; set; }
    }
}
