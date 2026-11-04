---
title: "IAccessibility"
description: "The Accessibility class provides methods for inspecting Chromium's accessibility tree. The accessibility tree is used by assistive technology such as screen..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IAccessibility"
  hidden: true
---

<a id="PuppeteerSharp_PageAccessibility_IAccessibility"></a>

Namespace: [PuppeteerSharp.PageAccessibility](/api/PuppeteerSharp.PageAccessibility/)


```csharp
public interface IAccessibility
```


The Accessibility class provides methods for inspecting Chromium's accessibility tree.
The accessibility tree is used by assistive technology such as screen readers or switches.

Accessibility is a very platform-specific thing. On different platforms, there are different screen readers that might have wildly different output.
Blink - Chrome's rendering engine - has a concept of "accessibility tree", which is than translated into different platform-specific APIs.
Accessibility namespace gives users access to the Blink Accessibility Tree.
Most of the accessibility tree gets filtered out when converting from Blink AX Tree to Platform-specific AX-Tree or by assistive technologies themselves.
By default, Puppeteer tries to approximate this filtering, exposing only the "interesting" nodes of the tree.

**Derived:** [Accessibility](/api/PuppeteerSharp.PageAccessibility/Accessibility/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/IAccessibility.cs#L15)
