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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc />
public class CdpElementHandle : ElementHandle
{
    private readonly CdpFrame _cdpFrame;

    internal CdpElementHandle(
        IsolatedWorld world,
        RemoteObject remoteObject) : base(world, remoteObject)
    {
        Handle = new CdpJSHandle(world, remoteObject);
        Logger = Realm.Environment.Client.Connection.LoggerFactory.CreateLogger(GetType());
        _cdpFrame = Realm.Frame as CdpFrame;
    }

    /// <summary>
    /// Logger.
    /// </summary>
    internal ILogger Logger { get; }

    internal override CustomQuerySelectorRegistry CustomQuerySelectorRegistry =>
        Client.Connection.CustomQuerySelectorRegistry;

    /// <inheritdoc/>
    protected override Page Page => _cdpFrame.FrameManager.Page;

    private CDPSession Client => Handle.Realm.Environment.Client;

    private FrameManager FrameManager => _cdpFrame.FrameManager;

    /// <inheritdoc/>
    public override async Task<IFrame> ContentFrameAsync()
    {
        var nodeInfo = await Client
            .SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest { ObjectId = Id, })
            .ConfigureAwait(false);

        return string.IsNullOrEmpty(nodeInfo.Node.FrameId)
            ? null
            : await FrameManager.FrameTree.GetFrameAsync(nodeInfo.Node.FrameId).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override Task ScrollIntoViewAsync()
        => BindIsolatedHandleAsync<IElementHandle, CdpElementHandle>(async handle =>
        {
            await handle.AssertConnectedElementAsync().ConfigureAwait(false);
            try
            {
                await handle.Client
                    .SendAsync(
                        "DOM.scrollIntoViewIfNeeded",
                        new DomScrollIntoViewIfNeededRequest { ObjectId = Id, })
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DOM.scrollIntoViewIfNeeded is not supported");
                await base.ScrollIntoViewAsync().ConfigureAwait(false);
            }

            return handle;
        });

    /// <inheritdoc />
    public override Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
            => BindIsolatedHandleAsync<CdpElementHandle, CdpElementHandle>(async handle =>
            {
                var isMultiple = await EvaluateFunctionAsync<bool>("element => element.multiple").ConfigureAwait(false);

                if (!isMultiple && filePaths.Length > 1)
                {
                    throw new PuppeteerException("Multiple file uploads only work with <input type=file multiple>");
                }

                // The zero-length array is a special case, it seems that
                // DOM.setFileInputFiles does not actually update the files in that case, so
                // the solution is to eval the element value to a new FileList directly.
                if (!filePaths.Any())
                {
                    await handle.EvaluateFunctionAsync(@"(element) => {
                        element.files = new DataTransfer().files;

                        // Dispatch events for this case because it should behave akin to a user action.
                        element.dispatchEvent(
                            new Event('input', {bubbles: true, composed: true})
                        );
                        element.dispatchEvent(new Event('change', { bubbles: true }));
                    }").ConfigureAwait(false);

                    return handle;
                }

                var node = await handle.Client
                    .SendAsync<DomDescribeNodeResponse>(
                        "DOM.describeNode",
                        new DomDescribeNodeRequest { ObjectId = Id, }).ConfigureAwait(false);
                var backendNodeId = node.Node.BackendNodeId;

                var files = resolveFilePaths ? filePaths.Select(Path.GetFullPath).ToArray() : filePaths;
                CheckForFileAccess(files);
                await handle.Client.SendAsync(
                    "DOM.setFileInputFiles",
                    new DomSetFileInputFilesRequest
                    {
                        ObjectId = Id,
                        Files = files,
                        BackendNodeId = backendNodeId,
                    })
                    .ConfigureAwait(false);

                return handle;
            });

    /// <inheritdoc />
    public override ValueTask DisposeAsync()
    {
        if (Disposed)
        {
            return default;
        }

        Disposed = true;

        Handle.DisposeAsync();
        GC.SuppressFinalize(this);
        return default;
    }

    /// <inheritdoc />
    public override string ToString() => Handle.ToString();

    private void CheckForFileAccess(string[] files)
    {
        foreach (var file in files)
        {
            try
            {
                File.Open(file, FileMode.Open).Dispose();
            }
            catch (Exception ex)
            {
                throw new PuppeteerException($"{files} does not exist or is not readable", ex);
            }
        }
    }
}
