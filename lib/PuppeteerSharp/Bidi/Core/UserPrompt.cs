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

using System;
using System.Threading.Tasks;
using WebDriverBiDi.BrowsingContext;
using WebDriverBiDi.Session;

namespace PuppeteerSharp.Bidi.Core;

/// <summary>
/// Represents a user prompt (dialog) in a browsing context.
/// </summary>
internal class UserPrompt(BrowsingContext browsingContext, UserPromptOpenedEventArgs info) : IDisposable
{
    private string _reason;
    private UserPromptClosedEventArgs _result;

    /// <summary>
    /// Gets the information about the user prompt when it was opened.
    /// </summary>
    public UserPromptOpenedEventArgs Info => info;

    /// <summary>
    /// Gets a value indicating whether the prompt has been closed.
    /// </summary>
    public bool Closed => _reason != null;

    /// <summary>
    /// Gets a value indicating whether the prompt has been handled.
    /// </summary>
    public bool Handled
    {
        get
        {
            // If the prompt has a handler configured (auto-accept/dismiss), it's considered handled
            if (Info.Handler == UserPromptHandlerType.Accept || Info.Handler == UserPromptHandlerType.Dismiss)
            {
                return true;
            }

            return _result != null;
        }
    }

    /// <summary>
    /// Gets the result of handling the prompt, if available.
    /// </summary>
    public UserPromptClosedEventArgs Result => _result;

    /// <summary>
    /// Creates a new UserPrompt instance and initializes event listeners.
    /// </summary>
    /// <param name="browsingContext">The browsing context.</param>
    /// <param name="info">The user prompt opened event args.</param>
    /// <returns>A new UserPrompt instance.</returns>
    public static UserPrompt From(BrowsingContext browsingContext, UserPromptOpenedEventArgs info)
    {
        var userPrompt = new UserPrompt(browsingContext, info);
        userPrompt.Initialize();
        return userPrompt;
    }

    /// <summary>
    /// Handles the user prompt by accepting or dismissing it.
    /// </summary>
    /// <param name="accept">Whether to accept or dismiss the prompt.</param>
    /// <param name="userText">Optional text to enter (for prompt dialogs).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(bool accept, string userText)
    {
        if (Closed)
        {
            throw new PuppeteerException($"User prompt already closed: {_reason}");
        }

        await browsingContext.Session.Driver.BrowsingContext.HandleUserPromptAsync(new HandleUserPromptCommandParameters(browsingContext.Id)
        {
            Accept = accept,
            UserText = userText,
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Disposes the user prompt and cleans up event listeners.
    /// </summary>
    public void Dispose()
    {
        _reason ??= "User prompt already closed, probably because the associated browsing context was destroyed.";
        GC.SuppressFinalize(this);
    }

    private void Initialize()
    {
        // Listen for the browsing context being closed
        browsingContext.Closed += OnBrowsingContextClosed;

        // Listen for the user prompt being closed
        browsingContext.Session.BrowsingContextUserPromptClosed += OnUserPromptClosed;
    }

    private void OnBrowsingContextClosed(object sender, ClosedEventArgs e)
    {
        Dispose($"User prompt already closed: {e.Reason}");
    }

    private void OnUserPromptClosed(object sender, UserPromptClosedEventArgs e)
    {
        if (e.BrowsingContextId != browsingContext.Id)
        {
            return;
        }

        _result = e;
        Dispose("User prompt already handled.");
    }

    private void Dispose(string reason)
    {
        if (Closed)
        {
            return;
        }

        _reason = reason;

        // Clean up event handlers
        browsingContext.Closed -= OnBrowsingContextClosed;
        browsingContext.Session.BrowsingContextUserPromptClosed -= OnUserPromptClosed;
    }
}

#endif
