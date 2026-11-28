---
title: "IWritableDestination"
description: "Represents a writable destination for a ScreenRecording."
editUrl: false
prev: false
next: false
sidebar:
  label: "IWritableDestination"
  hidden: true
---

<a id="PuppeteerSharp_IWritableDestination"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IWritableDestination
```


Represents a writable destination for a [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IWritableDestination.cs#L8)

## Methods

<a id="PuppeteerSharp_IWritableDestination_End_"></a>
<a id="PuppeteerSharp_IWritableDestination_End"></a>

### End()


```csharp
void End()
```


Ends the destination stream.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IWritableDestination.cs#L20)

<a id="PuppeteerSharp_IWritableDestination_Write_"></a>
<a id="PuppeteerSharp_IWritableDestination_Write_System_ReadOnlySpan_System_Byte__"></a>

### `Write(ReadOnlySpan<byte>)`


```csharp
bool Write(ReadOnlySpan<byte> chunk)
```


Writes a chunk of data to the destination.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `chunk` | [ReadOnlySpan&lt;byte&gt;](https://learn.microsoft.com/dotnet/api/system.readonlyspan-1) | The chunk to write. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the chunk was written.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IWritableDestination.cs#L15)
