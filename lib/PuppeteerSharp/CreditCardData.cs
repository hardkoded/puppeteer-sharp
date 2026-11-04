namespace PuppeteerSharp
{
    /// <summary>
    /// Credit card data for autofilling.
    /// See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-CreditCard.
    /// </summary>
    public class CreditCardData
    {
        /// <summary>
        /// Gets or sets the credit card number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the name on the credit card.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expiry month.
        /// </summary>
        public string ExpiryMonth { get; set; }

        /// <summary>
        /// Gets or sets the expiry year.
        /// </summary>
        public string ExpiryYear { get; set; }

        /// <summary>
        /// Gets or sets the CVC.
        /// </summary>
        public string Cvc { get; set; }
    }
}
