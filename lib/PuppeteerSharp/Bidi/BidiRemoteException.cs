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
using System.Linq;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

internal static class BidiRemoteException
{
    /// <summary>
    /// Creates a <see cref="RemoteException"/> from a WebDriver BiDi JavaScript log entry.
    /// WebDriver BiDi does not expose the exception name nor a remote reference to the thrown value,
    /// so <see cref="RemoteException.Name"/> is empty and <see cref="RemoteException.Handle"/> is null.
    /// </summary>
    /// <param name="text">The log entry text.</param>
    /// <param name="stackTrace">The log entry stack trace.</param>
    /// <returns>The remote exception.</returns>
    internal static RemoteException Create(string text, StackTrace stackTrace)
        => new(
            text ?? string.Empty,
            string.Empty,
            GetStackTrace(stackTrace),
            null,
            null);

    /// <summary>
    /// Creates the event args of the deprecated <c>PageError</c> event from a WebDriver BiDi JavaScript log entry.
    /// </summary>
    /// <param name="text">The log entry text.</param>
    /// <param name="stackTrace">The log entry stack trace.</param>
    /// <returns>The page error event args.</returns>
    internal static PageErrorEventArgs CreatePageError(string text, StackTrace stackTrace)
    {
        var stackLines = new List<string>();
        if (stackTrace?.CallFrames != null)
        {
            foreach (var frame in stackTrace.CallFrames)
            {
                stackLines.Add($"    at {(string.IsNullOrEmpty(frame.FunctionName) ? "<anonymous>" : frame.FunctionName)} ({frame.Url}:{frame.LineNumber + 1}:{frame.ColumnNumber + 1})");
                if (stackLines.Count >= 10)
                {
                    break;
                }
            }
        }

        var fullStack = string.Join("\n", new[] { text ?? string.Empty }.Concat(stackLines));
        return new PageErrorEventArgs(fullStack, fullStack, GetStackTrace(stackTrace));
    }

    private static IReadOnlyList<ConsoleMessageLocation> GetStackTrace(StackTrace stackTrace)
        => stackTrace?.CallFrames?.Select(callFrame => new ConsoleMessageLocation
        {
            URL = callFrame.Url,
            LineNumber = (int)callFrame.LineNumber,
            ColumnNumber = (int)callFrame.ColumnNumber,
            FunctionName = callFrame.FunctionName,
        }).ToList() ?? [];
}

#endif
