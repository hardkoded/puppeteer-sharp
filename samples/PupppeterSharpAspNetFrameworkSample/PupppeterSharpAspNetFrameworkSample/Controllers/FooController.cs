using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PupppeterSharpAspNetFrameworkSample.Controllers
{
    public class FooController : ApiController
    {
        public string Get() => "bar";
    }
}
