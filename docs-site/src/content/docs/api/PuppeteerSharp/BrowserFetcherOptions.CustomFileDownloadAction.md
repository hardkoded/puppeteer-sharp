---
title: "BrowserFetcherOptions.CustomFileDownloadAction"
description: "A custom download delegate."
editUrl: false
prev: false
next: false
sidebar:
  label: "BrowserFetcherOptions.CustomFileDownloadAction"
  hidden: true
---

<a id="PuppeteerSharp_BrowserFetcherOptions_CustomFileDownloadAction"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public delegate Task BrowserFetcherOptions.CustomFileDownloadAction(string address, string fileName)
```


A custom download delegate.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `address` | [string](https://learn.microsoft.com/dotnet/api/system.string) | address. |
| `fileName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | fileName. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the download finishes.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L16)
