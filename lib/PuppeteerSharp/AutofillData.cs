namespace PuppeteerSharp
{
    /// <summary>
    /// Data for autofilling form fields.
    /// Provide either <see cref="CreditCard"/> or <see cref="Address"/>, but not both.
    /// </summary>
    public class AutofillData
    {
        /// <summary>
        /// Gets or sets the credit card data.
        /// See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-CreditCard.
        /// </summary>
        public CreditCardData CreditCard { get; set; }

        /// <summary>
        /// Gets or sets the address data.
        /// See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address.
        /// </summary>
        public AutofillAddressData Address { get; set; }
    }
}
