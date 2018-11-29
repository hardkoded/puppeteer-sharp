using System.Threading.Tasks;
using System.Web.Http;
using PupppeterSharpAspNetFrameworkSample.Services;

namespace PupppeterSharpAspNetFrameworkSample.Controllers
{
    public class TestController : ApiController
    {
        public Task<string> Get() => BrowserClient.GetTextAsync("http://localhost/PupppeterSharpAspNetFrameworkSample/api/foo");
    }
}
