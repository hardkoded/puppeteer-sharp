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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.QueryHandlers;

namespace PuppeteerSharp.Cdp;

/// <inheritdoc cref="ElementHandle" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CdpElementHandle : ElementHandle, ICdpHandle
{
    private readonly CdpFrame _cdpFrame;
    private int? _backendNodeId;

    internal CdpElementHandle(
        IsolatedWorld world,
#pragma warning disable CA2000
        RemoteObject remoteObject) : base(new CdpJSHandle(world, remoteObject))
#pragma warning restore CA2000
    {
        Logger = world.CdpCDPSession.Connection.LoggerFactory.CreateLogger(GetType());
        _cdpFrame = IsolatedWorld.Frame as CdpFrame;
    }

    /// <summary>
    /// CDP Remote object.
    /// </summary>
    public RemoteObject RemoteObject => ((CdpJSHandle)Handle).RemoteObject;

    /// <inheritdoc/>
    internal override Realm Realm => Handle.Realm;

    /// <summary>
    /// Logger.
    /// </summary>
    internal ILogger Logger { get; }

    internal override CustomQuerySelectorRegistry CustomQuerySelectorRegistry => CustomQuerySelectorRegistry.Default;

    internal string Id => RemoteObject.ObjectId;

    /// <inheritdoc/>
    protected override Page Page => _cdpFrame.FrameManager.Page;

    private IsolatedWorld IsolatedWorld => (IsolatedWorld)Realm;

    private ICDPSession Client => Handle.Realm.Environment.Client;

    private FrameManager FrameManager => _cdpFrame.FrameManager;

    private string DebuggerDisplay =>
        string.IsNullOrEmpty(RemoteObject.ClassName)
            ? ToString()
            : $"{RemoteObject.ClassName}@{RemoteObject.Description}";

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
                if (filePaths.Length == 0)
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

    /// <inheritdoc/>
    public override async Task<int> BackendNodeIdAsync()
    {
        if (_backendNodeId.HasValue)
        {
            return _backendNodeId.Value;
        }

        var response = await Client
            .SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest { ObjectId = Id, })
            .ConfigureAwait(false);

        _backendNodeId = response.Node.BackendNodeId.GetInt32();
        return _backendNodeId.Value;
    }

    /// <inheritdoc/>
    public override async Task AutofillAsync(AutofillData data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var nodeInfo = await Client
            .SendAsync<DomDescribeNodeResponse>("DOM.describeNode", new DomDescribeNodeRequest { ObjectId = Id, })
            .ConfigureAwait(false);
        var fieldId = nodeInfo.Node.BackendNodeId.GetInt32();
        var frameId = _cdpFrame.Id;
        await Client.SendAsync(
            "Autofill.trigger",
            new AutofillTriggerRequest
            {
                FieldId = fieldId,
                FrameId = frameId,
                Card = data.CreditCard,
            }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override string ToString() => Handle.ToString();
}
