// * MIT License
//  *
//  * Copyright (c) Microsoft Corporation.
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp;

/// <summary>
/// This is a DisposableStack in puppeteer. We don't need to dispose objects manually.
/// But this is used to run actions when this object is disposed.
/// </summary>
internal class DisposableTasksStack : IAsyncDisposable, IDisposable
{
    private readonly List<Func<Task>> _tasks = [];

    public bool IsDisposed { get; private set; }

    public void Defer(Func<Task> task)
    {
        _tasks.Add(task);
    }

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        _tasks.Reverse();

        foreach (var task in _tasks)
        {
            await task().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _ = DisposeAsync();
    }
}
