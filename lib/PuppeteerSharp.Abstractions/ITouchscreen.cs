using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    public interface ITouchscreen
    {
        Task TapAsync(decimal x, decimal y);
    }
}