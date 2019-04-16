using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Abstractions.Input;

namespace PuppeteerSharp.Abstractions
{
    interface IElementHandle
    {
        Task ScreenshotAsync(string file);
        Task ScreenshotAsync(string file, ScreenshotOptions options);
        Task<Stream> ScreenshotStreamAsync();
        Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options);
        Task<byte[]> ScreenshotDataAsync();
        Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options);
        Task<string> ScreenshotBase64Async();
        Task<string> ScreenshotBase64Async(ScreenshotOptions options);
        Task HoverAsync();
        Task ClickAsync(ClickOptions options = null);
        Task UploadFileAsync(params string[] filePaths);
        Task TapAsync();
        Task FocusAsync();
        Task TypeAsync(string text, TypeOptions options = null);
        Task PressAsync(string key, PressOptions options = null);
        Task<IElementHandle> QuerySelectorAsync(string selector);
        Task<IElementHandle[]> QuerySelectorAllAsync(string selector);
        Task<IJSHandle> QuerySelectorAllHandleAsync(string selector);
        Task<IElementHandle[]> XPathAsync(string expression);
        Task<BoundingBox> BoundingBoxAsync();
        Task<BoxModel> BoxModelAsync();
        Task<IFrame> ContentFrameAsync();
        Task<bool> IsIntersectingViewportAsync();
    }

}
