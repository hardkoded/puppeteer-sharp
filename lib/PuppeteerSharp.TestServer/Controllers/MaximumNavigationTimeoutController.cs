using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.TestServer.Controllers
{
    public class MaximumNavigationTimeoutController : Controller
    {
        [HttpGet("maximumnavigationtimeout/testuser")]
        public async Task<IActionResult> Test()
        {
            await Task.Delay(TimeSpan.FromSeconds(30));

            return Ok();
        }
    }
}
