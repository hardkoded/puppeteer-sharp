---
title: "PuppeteerSharp.Input"
description: "Keyboard, mouse, and touch input."
editUrl: false
sidebar:
  label: "PuppeteerSharp.Input"
---

[API reference](/api/)

Keyboard, mouse, and touch input.

## Classes

| Type | Summary |
| --- | --- |
| [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to use when clicking. |
| [DownOptions](/api/PuppeteerSharp.Input/DownOptions/) | options to use with DownOptions). |
| [Key](/api/PuppeteerSharp.Input/Key/) | Utility class to be used with Keyboard operations. |
| [Keyboard](/api/PuppeteerSharp.Input/Keyboard/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [Mouse](/api/PuppeteerSharp.Input/Mouse/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [MoveOptions](/api/PuppeteerSharp.Input/MoveOptions/) | options to use MoveOptions). |
| [PressOptions](/api/PuppeteerSharp.Input/PressOptions/) | options to use when pressing a key. |
| [Touchscreen](/api/PuppeteerSharp.Input/Touchscreen/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [TypeOptions](/api/PuppeteerSharp.Input/TypeOptions/) | Options to use when typing. |

## Interfaces

| Type | Summary |
| --- | --- |
| [IKeyboard](/api/PuppeteerSharp.Input/IKeyboard/) | Keyboard provides an api for managing a virtual keyboard. The high level api is TypeOptions), which takes raw characters and generates proper keydown, keypress/input, and keyup events on your page. For finer control, you can use DownOptions), UpAsync(string), and SendCharacterAsync(string) to manually fire events as if they were generated from a real keyboard. |
| [IMouse](/api/PuppeteerSharp.Input/IMouse/) | Provides methods to interact with the mouse. |
| [ITouchHandle](/api/PuppeteerSharp.Input/ITouchHandle/) | The TouchHandle interface exposes methods to manipulate touches that have been started. |
| [ITouchscreen](/api/PuppeteerSharp.Input/ITouchscreen/) | Provides methods to interact with the touch screen. |

## Enums

| Type | Summary |
| --- | --- |
| [MouseButton](/api/PuppeteerSharp.Input/MouseButton/) | The type of button click to use with DownAsync(ClickOptions), UpAsync(ClickOptions) and ClickOptions). |
