// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.QueryHandlers;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

#pragma warning disable CA2000
internal class BidiElementHandle(RemoteValue value, BidiRealm realm) : ElementHandle(BidiJSHandle.From(value, realm))
#pragma warning restore CA2000
{
    /// <summary>
    /// Bidi Remote value.
    /// </summary>
    public RemoteValue Value { get; } = value;

    internal BidiJSHandle BidiJSHandle => Handle as BidiJSHandle;

    internal override Realm Realm => realm;

    internal override CustomQuerySelectorRegistry CustomQuerySelectorRegistry => CustomQuerySelectorRegistry.Default;

    internal BidiFrame BidiFrame => realm.Environment as BidiFrame;

    protected override Page Page => BidiFrame.BidiPage;

    public static IJSHandle From(RemoteValue value, BidiRealm realm)
    {
        return new BidiElementHandle(value, realm);
    }

    public override async Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
    {
        // Resolve file paths to absolute paths if needed
        if (resolveFilePaths)
        {
            filePaths = filePaths.Select(file =>
            {
                if (Path.IsPathRooted(file))
                {
                    return file;
                }

                return Path.GetFullPath(file);
            }).ToArray();
        }

        await BidiFrame.SetFilesAsync(this, filePaths).ConfigureAwait(false);
    }

    public override async Task<IFrame> ContentFrameAsync()
    {
        var handle = await EvaluateFunctionHandleAsync(@"element => {
            if (element instanceof HTMLIFrameElement || element instanceof HTMLFrameElement) {
                return element.contentWindow;
            }
            return;
        }").ConfigureAwait(false);

        // Get the RemoteValue from either BidiJSHandle or BidiElementHandle (which wraps BidiJSHandle)
        RemoteValue value = handle switch
        {
            BidiElementHandle bidiElement => bidiElement.BidiJSHandle.RemoteValue,
            BidiJSHandle bidiJsHandle => bidiJsHandle.RemoteValue,
            _ => null,
        };

        await handle.DisposeAsync().ConfigureAwait(false);

        if (value?.Type == "window" && value.Value is WindowProxyProperties windowProxy)
        {
            var contextId = windowProxy.Context;
            return BidiFrame.BidiPage.Frames.FirstOrDefault(frame => frame.Id == contextId);
        }

        return null;
    }

    internal async IAsyncEnumerable<IElementHandle> QueryAXTreeAsync(string name, string role)
    {
        var locator = new AccessibilityLocator { Name = name, Role = role };
        var results = await BidiFrame.LocateNodesAsync(this, locator).ConfigureAwait(false);

        foreach (var node in results)
        {
            yield return new BidiElementHandle(node, realm);
        }
    }
}

#endif
