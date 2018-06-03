using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    /// <summary>
    /// Security details.
    /// </summary>
    /// <seealso cref="Response.SecurityDetails"/>
    public class SecurityDetails
    {
        internal SecurityDetails(string subjectName, string issuer, int validFrom, int validTo, string protocol)
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
        public string SubjectName { get; }
        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer { get; }
        /// <summary>
        /// Gets the valid from.
        /// </summary>
        /// <value>The valid from.</value>
        public int ValidFrom { get; }
        /// <summary>
        /// Gets the valid to.
        /// </summary>
        /// <value>The valid to.</value>
        public int ValidTo { get; }
        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public string Protocol { get; }
    }
}