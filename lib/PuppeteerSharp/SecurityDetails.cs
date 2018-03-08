using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    public class SecurityDetails
    {
        public SecurityDetails(string subjectName, string issuer, int validFrom, int validTo, string protocol)
        {
            SubjectName = subjectName;
            Issuer = issuer;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Protocol = protocol;
        }

        public string SubjectName { get; }
        public string Issuer { get; }
        public int ValidFrom { get; }
        public int ValidTo { get; }
        public string Protocol { get; }

    }
}