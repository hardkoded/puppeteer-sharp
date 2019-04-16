using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface ITouchscreen
    {
        Task TapAsync(decimal x, decimal y);
    }
}