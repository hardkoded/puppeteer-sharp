using System;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Input;
using WebDriverBiDi.Input;

namespace PuppeteerSharp.Bidi;

internal class BidiTouchscreen(BidiPage page) : Touchscreen
{
    private readonly PointerSourceActions _touchSource = new()
    {
        PointerType = PointerType.Touch
    };
    private readonly TaskQueue _actionsQueue = new();

    public override Task TouchStartAsync(decimal x, decimal y)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });
            _touchSource.Actions.Add(new PointerDownAction(0));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    public override Task TouchMoveAsync(decimal x, decimal y)
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerMoveAction
            {
                X = (long)Math.Round(x),
                Y = (long)Math.Round(y),
            });

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }

    public override Task TouchEndAsync()
    {
        return _actionsQueue.Enqueue(async () =>
        {
            _touchSource.Actions.Add(new PointerUpAction(0));

            await page.BidiMainFrame.BrowsingContext.PerformActionsAsync([_touchSource]).ConfigureAwait(false);
            _touchSource.Actions.Clear();
        });
    }
}
