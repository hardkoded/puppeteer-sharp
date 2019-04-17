namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the security details when response was received over the secure connection.
    /// </summary>
    /// <seealso cref="Response.SecurityDetails"/>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.SecurityDetails class instead")]
    public class SecurityDetails : Abstractions.SecurityDetails
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="SecurityDetails"/> class.
        /// </summary>
        public SecurityDetails()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityDetails"/> class.
        /// </summary>
        /// <param name="subjectName">Subject name</param>
        /// <param name="issuer">Issuer</param>
        /// <param name="validFrom">Valid from</param>
        /// <param name="validTo">Valid to</param>
        /// <param name="protocol">Protocol</param>
        public SecurityDetails(string subjectName, string issuer, int validFrom, int validTo, string protocol)
            : base(subjectName, issuer, validFrom, validTo, protocol)
        {
        }
    }
}