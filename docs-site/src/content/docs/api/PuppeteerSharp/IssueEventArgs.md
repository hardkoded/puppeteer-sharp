---
title: "IssueEventArgs"
description: "Issue event arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "IssueEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_IssueEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class IssueEventArgs : EventArgs
```


[Issue](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Issue) event arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IssueEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_IssueEventArgs__ctor_"></a>
<a id="PuppeteerSharp_IssueEventArgs__ctor_PuppeteerSharp_Issue_"></a>

### IssueEventArgs(Issue)


```csharp
public IssueEventArgs(Issue issue)
```


Initializes a new instance of the [IssueEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `issue` | [Issue](/api/PuppeteerSharp/Issue/) | The issue that was reported. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IssueEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_IssueEventArgs_Issue_"></a>
<a id="PuppeteerSharp_IssueEventArgs_Issue"></a>

### Issue


```csharp
public Issue Issue { get; }
```


Gets the reported issue.

**Value:** [Issue](/api/PuppeteerSharp/Issue/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IssueEventArgs.cs#L19)
