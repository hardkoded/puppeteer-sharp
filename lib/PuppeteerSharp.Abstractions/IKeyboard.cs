using System.Threading.Tasks;
using PuppeteerSharp.Abstractions.Input;

namespace PuppeteerSharp.Abstractions
{
    public interface IKeyboard
    {
        Task DownAsync(string key, DownOptions options = null);
        Task UpAsync(string key);
        Task SendCharacterAsync(string charText);
        Task TypeAsync(string text, TypeOptions options = null);
        Task PressAsync(string key, PressOptions options = null);
    }
}
