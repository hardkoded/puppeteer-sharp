---
title: "PuppeteerSharp.PageAccessibility"
description: "Accessibility tree snapshots."
editUrl: false
sidebar:
  label: "PuppeteerSharp.PageAccessibility"
---

[API reference](/api/)

Accessibility tree snapshots.

## Classes

| Type | Summary |
| --- | --- |
| [Accessibility](/api/PuppeteerSharp.PageAccessibility/Accessibility/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [AccessibilitySnapshotOptions](/api/PuppeteerSharp.PageAccessibility/AccessibilitySnapshotOptions/) | SnapshotAsync(AccessibilitySnapshotOptions). |
| [SerializedAXNode](/api/PuppeteerSharp.PageAccessibility/SerializedAXNode/) | AXNode. |

## Interfaces

| Type | Summary |
| --- | --- |
| [IAccessibility](/api/PuppeteerSharp.PageAccessibility/IAccessibility/) | The Accessibility class provides methods for inspecting Chromium's accessibility tree. The accessibility tree is used by assistive technology such as screen readers or switches. Accessibility is a very platform-specific thing. On different platforms, there are different screen readers that might have wildly different output. Blink - Chrome's rendering engine - has a concept of "accessibility tree", which is than translated into different platform-specific APIs. Accessibility namespace gives users access to the Blink Accessibility Tree. Most of the accessibility tree gets filtered out when converting from Blink AX Tree to Platform-specific AX-Tree or by assistive technologies themselves. By default, Puppeteer tries to approximate this filtering, exposing only the "interesting" nodes of the tree. |

## Enums

| Type | Summary |
| --- | --- |
| [CheckedState](/api/PuppeteerSharp.PageAccessibility/CheckedState/) | Three-state boolean. See Checked and. Pressed |
