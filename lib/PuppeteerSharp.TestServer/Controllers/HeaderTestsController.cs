using System;
using Microsoft.AspNetCore.Mvc;

namespace PuppeteerSharp.TestServer.Controllers
{
    public class HeaderTestsController : Controller
    {
        [HttpGet("headertests/test")]
        public string Test()
        {
            return HttpContext.Request.Headers["Foo"];
        }
    }
}
