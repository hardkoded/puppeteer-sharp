---
title: "InstalledBrowser"
description: "Installed browser info."
editUrl: false
prev: false
next: false
sidebar:
  label: "InstalledBrowser"
  hidden: true
---

<a id="PuppeteerSharp_BrowserData_InstalledBrowser"></a>

Namespace: [PuppeteerSharp.BrowserData](/api/PuppeteerSharp.BrowserData/)


```csharp
public class InstalledBrowser
```


Installed browser info.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L9)

## Properties

<a id="PuppeteerSharp_BrowserData_InstalledBrowser_Browser_"></a>
<a id="PuppeteerSharp_BrowserData_InstalledBrowser_Browser"></a>

### Browser


```csharp
public SupportedBrowser Browser { get; set; }
```


Browser.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L29)

<a id="PuppeteerSharp_BrowserData_InstalledBrowser_BuildId_"></a>
<a id="PuppeteerSharp_BrowserData_InstalledBrowser_BuildId"></a>

### BuildId


```csharp
public string BuildId { get; set; }
```


Gets or sets the buildID.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L34)

<a id="PuppeteerSharp_BrowserData_InstalledBrowser_PermissionsFixed_"></a>
<a id="PuppeteerSharp_BrowserData_InstalledBrowser_PermissionsFixed"></a>

### PermissionsFixed


```csharp
public bool? PermissionsFixed { get; }
```


Whether the permissions have been fixed in the browser.
If Puppeteer executed the command to fix the permissions, this will be true.
If Puppeteer failed to fix the permissions, this will be false.
If the platform does not require permissions to be fixed, this will be null.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L47)

<a id="PuppeteerSharp_BrowserData_InstalledBrowser_Platform_"></a>
<a id="PuppeteerSharp_BrowserData_InstalledBrowser_Platform"></a>

### Platform


```csharp
public Platform Platform { get; set; }
```


Revision platform.

**Value:** [Platform](/api/PuppeteerSharp/Platform/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L39)

## Methods

<a id="PuppeteerSharp_BrowserData_InstalledBrowser_GetExecutablePath_"></a>
<a id="PuppeteerSharp_BrowserData_InstalledBrowser_GetExecutablePath"></a>

### GetExecutablePath()


```csharp
public string GetExecutablePath()
```


Get executable path.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — executable path.

**Exceptions**

| Type | Condition |
| --- | --- |
| [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception) | For not supported [Platform](#PuppeteerSharp_BrowserData_InstalledBrowser_Platform). |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserData/InstalledBrowser.cs#L59)
