using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Cdp;

internal static class CdpRemoteException
{
    internal static RemoteObject ToRemoteObject(EvaluateExceptionResponseInfo info)
        => new()
        {
            ObjectId = info.ObjectId,
            Type = info.Type,
            Subtype = info.Subtype,
            Description = info.Description,
        };

    internal static RemoteException Create(EvaluateExceptionResponseDetails details, IJSHandle handle)
    {
        string text;
        string name;
        var exception = details.Exception;

        if (exception == null)
        {
            // The browser reports no exception value for exceptions thrown by code that
            // Puppeteer injected.
            text = details.Text;
            name = "Error";
        }
        else if (exception.Type == RemoteObjectType.Object && exception.Subtype == RemoteObjectSubtype.Error)
        {
            // The description is the error as rendered by the browser followed by the stack, if any.
            text = (exception.Description ?? string.Empty).Split(["\n    at "], System.StringSplitOptions.None)[0];
            var header = text.Split('\n')[0];
            var separator = header.IndexOf(": ", System.StringComparison.Ordinal);
            name = separator == -1 ? header : header.Substring(0, separator);
        }
        else if (!string.IsNullOrEmpty(exception.ObjectId))
        {
            // A non-error object or a symbol was thrown.
            text = exception.Description ?? string.Empty;
            name = string.Empty;
        }
        else
        {
            // A primitive value was thrown.
            text = exception.Description ??
                exception.Value ??
                (exception.Subtype == RemoteObjectSubtype.Null ? "null" : "undefined");
            name = string.Empty;
        }

        IReadOnlyList<ConsoleMessageLocation> stackTrace = details.StackTrace?.CallFrames?
            .Select(frame => new ConsoleMessageLocation
            {
                URL = frame.Url,
                LineNumber = frame.LineNumber,
                ColumnNumber = frame.ColumnNumber,
                FunctionName = frame.FunctionName,
            })
            .ToList() ?? [];

        return new RemoteException(
            text,
            name,
            stackTrace,
            new ConsoleMessageLocation
            {
                URL = details.Url,
                LineNumber = details.LineNumber,
                ColumnNumber = details.ColumnNumber,
            },
            handle);
    }
}
