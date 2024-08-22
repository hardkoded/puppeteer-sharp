namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the security details when response was received over the secure connection.
    /// </summary>
    /// <seealso cref="IResponse.SecurityDetails"/>
    public class SecurityDetails
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
        /// <param name="subjectName">Subject name.</param>
        /// <param name="issuer">Issuer.</param>
        /// <param name="validFrom">Valid from.</param>
        /// <param name="validTo">Valid to.</param>
        /// <param name="protocol">Protocol.</param>
        public SecurityDetails(string subjectName, string issuer, long validFrom, long validTo, string protocol)
        {
            SubjectName = subjectName;
            Issuer = issuer;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Protocol = protocol;
        }

        /// <summary>
        /// Gets the name of the subject.
        /// </summary>
        /// <value>The name of the subject.</value>
        public string SubjectName { get; set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets the valid from.
        /// </summary>
        /// <value>The valid from.</value>
        public long ValidFrom { get; set; }

        /// <summary>
        /// Gets the valid to.
        /// </summary>
        /// <value>The valid to.</value>
        public long ValidTo { get; set; }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public string Protocol { get; set; }
    }
}
