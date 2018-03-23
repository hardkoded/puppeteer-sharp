namespace PuppeteerSharp.Tests
{
    public static class GoldenUtils
    {
        public static readonly string ReconnectNestedFramesTxt =
@"http://localhost:<PORT>/frames/nested-frames.html
    http://localhost:<PORT>/frames/two-frames.html
        http://localhost:<PORT>/frames/frame.html
        http://localhost:<PORT>/frames/frame.html
    http://localhost:<PORT>/frames/frame.html;";
    }
}
