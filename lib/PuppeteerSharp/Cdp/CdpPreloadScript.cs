// * MIT License
//  *
//  * Copyright (c) Dario Kondratiuk
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

using System.Runtime.CompilerServices;

namespace PuppeteerSharp.Cdp;

/// <summary>
/// Represents a preload script that is added via Page.addScriptToEvaluateOnNewDocument.
/// This tracks the CDP identifiers for the script across multiple frames,
/// since out-of-process frames get their own CDP session and identifier.
/// </summary>
internal class CdpPreloadScript
{
    private readonly ConditionalWeakTable<CdpFrame, StrongBox<string>> _frameToId = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CdpPreloadScript"/> class.
    /// </summary>
    /// <param name="mainFrame">The main frame where the script was first registered.</param>
    /// <param name="id">The CDP identifier returned by Page.addScriptToEvaluateOnNewDocument.</param>
    /// <param name="source">The source code of the script.</param>
    internal CdpPreloadScript(CdpFrame mainFrame, string id, string source)
    {
        Id = id;
        Source = source;
        SetIdForFrame(mainFrame, id);
    }

    /// <summary>
    /// Gets the ID of the preload script returned by Page.addScriptToEvaluateOnNewDocument
    /// in the main frame. Sub-frames would get a different CDP ID because
    /// addScriptToEvaluateOnNewDocument is called for each subframe. But
    /// users only see this ID and subframe IDs are internal to Puppeteer.
    /// </summary>
    internal string Id { get; }

    /// <summary>
    /// Gets the source code of the preload script.
    /// </summary>
    internal string Source { get; }

    /// <summary>
    /// Gets the CDP identifier for this script in the given frame.
    /// </summary>
    /// <param name="frame">The frame to look up.</param>
    /// <returns>The CDP identifier, or null if not registered for this frame.</returns>
    internal string GetIdForFrame(CdpFrame frame)
    {
        if (_frameToId.TryGetValue(frame, out var box))
        {
            return box.Value;
        }

        return null;
    }

    /// <summary>
    /// Sets the CDP identifier for this script in the given frame.
    /// </summary>
    /// <param name="frame">The frame to register.</param>
    /// <param name="identifier">The CDP identifier for this frame.</param>
    internal void SetIdForFrame(CdpFrame frame, string identifier)
    {
        _frameToId.Remove(frame);
        _frameToId.Add(frame, new StrongBox<string>(identifier));
    }
}
