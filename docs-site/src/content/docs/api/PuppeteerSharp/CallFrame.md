---
title: "CallFrame"
description: "Represents a call frame in a stack trace."
editUrl: false
prev: false
next: false
sidebar:
  label: "CallFrame"
  hidden: true
---

<a id="PuppeteerSharp_CallFrame"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CallFrame
```


Represents a call frame in a stack trace.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L28)

## Properties

<a id="PuppeteerSharp_CallFrame_ColumnNumber_"></a>
<a id="PuppeteerSharp_CallFrame_ColumnNumber"></a>

### ColumnNumber


```csharp
public int ColumnNumber { get; set; }
```


Gets or sets the column number in the script (0-based).

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L53)

<a id="PuppeteerSharp_CallFrame_FunctionName_"></a>
<a id="PuppeteerSharp_CallFrame_FunctionName"></a>

### FunctionName


```csharp
public string FunctionName { get; set; }
```


Gets or sets the JavaScript function name.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L33)

<a id="PuppeteerSharp_CallFrame_LineNumber_"></a>
<a id="PuppeteerSharp_CallFrame_LineNumber"></a>

### LineNumber


```csharp
public int LineNumber { get; set; }
```


Gets or sets the line number in the script (0-based).

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L48)

<a id="PuppeteerSharp_CallFrame_ScriptId_"></a>
<a id="PuppeteerSharp_CallFrame_ScriptId"></a>

### ScriptId


```csharp
public string ScriptId { get; set; }
```


Gets or sets the script identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L38)

<a id="PuppeteerSharp_CallFrame_Url_"></a>
<a id="PuppeteerSharp_CallFrame_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets or sets the URL of the script.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CallFrame.cs#L43)
