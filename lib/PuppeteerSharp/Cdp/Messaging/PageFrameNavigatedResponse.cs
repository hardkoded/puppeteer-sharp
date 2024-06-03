namespace PuppeteerSharp.Cdp.Messaging;

internal class PageFrameNavigatedResponse
{
    public FramePayload Frame { get; set; }

    public NavigationType Type { get; set; }
}
