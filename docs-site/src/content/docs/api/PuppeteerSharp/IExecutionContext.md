---
title: "IExecutionContext"
description: "The class represents a context for JavaScript execution. Examples of JavaScript contexts are: Each Frame has a separate IExecutionContext All kind of web wor..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IExecutionContext"
  hidden: true
---

<a id="PuppeteerSharp_IExecutionContext"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IExecutionContext
```


The class represents a context for JavaScript execution. Examples of JavaScript contexts are:
Each [Frame](#PuppeteerSharp_IExecutionContext_Frame) has a separate [IExecutionContext](./)
All kind of web workers have their own contexts.

**Derived:** [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IExecutionContext.cs#L11)
