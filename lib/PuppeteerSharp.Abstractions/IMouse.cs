using System.Threading.Tasks;
using PuppeteerSharp.Abstractions.Input;

namespace PuppeteerSharp.Abstractions
{
    interface IMouse
    {
        Task MoveAsync(decimal x, decimal y, MoveOptions options = null);
        Task ClickAsync(decimal x, decimal y, ClickOptions options = null);
        Task DownAsync(ClickOptions options = null);
        Task UpAsync(ClickOptions options = null);
        Task WheelAsync(decimal deltaX, decimal deltaY);
    }

}
