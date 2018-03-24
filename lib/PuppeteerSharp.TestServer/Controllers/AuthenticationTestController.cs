using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace PuppeteerSharp.TestServer.Controllers
{
    public class AuthenticationTestController : Controller
    {
        [HttpGet("authenticationtest/testuser")]
        [HttpGet("authenticationtest/testuser2")]
        [HttpGet("authenticationtest/testuser3")]
        public IActionResult TestUser()
        {
            if (!Authenticate("user", "pass"))
            {
                return Unauthorized();
            }
            return Ok("Ok");
        }

        private bool Authenticate(string usernameToCheck, string passwordToCheck)
        {
            string authHeader = Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic", StringComparison.Ordinal))
            {
                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                var encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                return username == usernameToCheck && password == passwordToCheck;
            }

            return false;
        }
    }
}
