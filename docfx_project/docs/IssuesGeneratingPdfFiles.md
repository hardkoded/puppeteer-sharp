# Issues generating PDF files
_Contributors: [Dario Kondratiuk](https://github.com/kblok)_

## Symptoms

When generating PDF files using `PdfAsync`, everything works fine in your development environment. However, once you deploy your application to a server, you get a timeout.

## Problem

Since Chromium version 125, generating PDF files requires sandbox permissions. [See chromium issue](https://issues.chromium.org/issues/338553158).
PuppeteerSharp tries to apply these permissions after the browser is downloaded, but it will proceed if this step fails. We want to ensure your application does not break if it never reaches the point of generating a PDF file.

## Solution

Verify whether PuppeteerSharp successfully applied the sandbox permissions. This can be done by checking the `InstalledBrowser.PermissionsFixed` property.

```csharp
var browserFetcher = new BrowserFetcher();
var installedBrowser = await browserFetcher.DownloadAsync(BrowserFetcher.);

if (!installedBrowser.PermissionsFixed)
{
    Console.WriteLine("Sandbox permissions were not applied. You need to run your application as an administrator.");
    return;
}
```

If PuppeteerSharp did not manage to apply the sandbox permissions, you can manually fix this by running the `setup.exe` file that was downloaded with the browser:

**IMPORTANT**: You need to run this as administrator.

```bash
cd <path-to-browser>
.\setup.exe --configure-browser-in-directory="<path-to-browser>"
```

If that doesn't work. You can try by fixing the permissions manually. You can find the instructions [here](https://pptr.dev/troubleshooting#chrome-reports-sandbox-errors-on-windows).

## Recommended approach

Installing the browser during runtime is not recommended, as it takes time and can delay your application. It is advisable to install the browser beforehand and pass the path to the `LaunchAsync` method.
