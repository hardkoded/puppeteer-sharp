---
title: "FileChooser"
description: "FileChooser objects are returned via the WaitForFileChooserAsync(WaitForOptions) method. File choosers let you react to the page requesting for a file."
editUrl: false
prev: false
next: false
sidebar:
  label: "FileChooser"
  hidden: true
---

<a id="PuppeteerSharp_FileChooser"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class FileChooser
```


[FileChooser](./) objects are returned via the [WaitForFileChooserAsync(WaitForOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForFileChooserAsync_PuppeteerSharp_WaitForOptions_) method.
File choosers let you react to the page requesting for a file.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

In browsers, only one file chooser can be opened at a time.
All file choosers must be accepted or canceled. Not doing so will prevent subsequent file choosers from appearing.

**Example**

```csharp
var waitTask = page.WaitForFileChooserAsync();
await Task.WhenAll(
    waitTask,
    page.ClickAsync("#upload-file-button")); // some button that triggers file selection

await waitTask.Result.AcceptAsync('/tmp/myfile.pdf');
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FileChooser.cs#L28)

## Properties

<a id="PuppeteerSharp_FileChooser_IsMultiple_"></a>
<a id="PuppeteerSharp_FileChooser_IsMultiple"></a>

### IsMultiple


```csharp
public bool IsMultiple { get; }
```


Whether file chooser allow for &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#attr-multiple"&gt;multiple&lt;/a&gt; file selection.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FileChooser.cs#L50)

## Methods

<a id="PuppeteerSharp_FileChooser_AcceptAsync_"></a>
<a id="PuppeteerSharp_FileChooser_AcceptAsync_System_String___"></a>

### AcceptAsync(params string[])


```csharp
public Task AcceptAsync(params string[] filePaths)
```


Accept the file chooser request with given paths.
If some of the filePaths are relative paths, then they are resolved relative to the current working directory.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | File paths to send to the file chooser. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves after the accept message is processed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FileChooser.cs#L58)

<a id="PuppeteerSharp_FileChooser_CancelAsync_"></a>
<a id="PuppeteerSharp_FileChooser_CancelAsync"></a>

### CancelAsync()


```csharp
public Task CancelAsync()
```


Closes the file chooser without selecting any files.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves after the cancel event is sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FileChooser.cs#L74)
