using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// Represents the security details when response was received over the secure connection.
    /// </summary>
    /// <seealso cref="Response.SecurityDetails"/>
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
        /// <param name="subjectName">Subject name</param>
        /// <param name="issuer">Issuer</param>
        /// <param name="validFrom">Valid from</param>
        /// <param name="validTo">Valid to</param>
        /// <param name="protocol">Protocol</param>
        public SecurityDetails(string subjectName, string issuer, int validFrom, int validTo, string protocol)
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
        [JsonProperty]
        public string SubjectName { get; internal set; }
        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        [JsonProperty]
        public string Issuer { get; internal set; }
        /// <summary>
        /// Gets the valid from.
        /// </summary>
        /// <value>The valid from.</value>
        [JsonProperty]
        public int ValidFrom { get; internal set; }
        /// <summary>
        /// Gets the valid to.
        /// </summary>
        /// <value>The valid to.</value>
        [JsonProperty]
        public int ValidTo { get; internal set; }
        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        [JsonProperty]
        public string Protocol { get; internal set; }
    }
}
