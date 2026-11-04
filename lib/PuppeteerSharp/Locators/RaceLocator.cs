using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PuppeteerSharp.Locators
{
    internal class RaceLocator : Locator
    {
        private readonly IReadOnlyList<Locator> _locators;

        internal RaceLocator(IReadOnlyList<Locator> locators)
        {
            _locators = locators;
        }

        internal override async Task<IJSHandle> WaitHandleCoreAsync(
            LocatorActionOptions options,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<IJSHandle>(TaskCreationOptions.RunContinuationsAsynchronously);
            var remaining = _locators.Count;
            Exception lastException = null;

            using var reg = cancellationToken.Register(() =>
                tcs.TrySetCanceled(cancellationToken));

            var tasks = _locators.Select(locator => Task.Run(async () =>
            {
                try
                {
                    var handle = await locator.WaitHandleCoreAsync(options, cancellationToken).ConfigureAwait(false);
                    if (!tcs.TrySetResult(handle))
                    {
                        // Another locator won; dispose this handle.
                        await handle.DisposeAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (Interlocked.Decrement(ref remaining) == 0)
                    {
                        tcs.TrySetException(lastException);
                    }
                }
            })).ToArray();

            return await tcs.Task.ConfigureAwait(false);
        }
    }
}
