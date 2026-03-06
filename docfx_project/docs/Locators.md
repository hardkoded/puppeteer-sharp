# Getting Started with Locators

Locators are the recommended way to find and interact with elements in Puppeteer Sharp. They provide built-in auto-retry and auto-wait functionality, making your automation scripts more reliable.

## What are Locators?

A Locator represents a way to find element(s) on a page. Every time an action is performed on a locator, the element is re-located, which helps avoid stale element references. Before performing actions, locators automatically check preconditions like visibility, enabled state, and stable positioning.

## Creating Locators

### CSS Selector Locator

Use `Page.Locator()` with a CSS selector to create a locator:

```cs
var locator = page.Locator("button.submit");
await locator.ClickAsync();
```

### Function Locator

Use `Page.LocatorFunction()` to create a locator that evaluates a JavaScript function:

```cs
var locator = page.LocatorFunction("() => document.querySelector('button')");
await locator.ClickAsync();
```

### Frame Locators

Locators also work with frames:

```cs
var frame = page.MainFrame.ChildFrames[0];
await frame.Locator("button.submit").ClickAsync();
```

## Configuration

Locators use a fluent API for configuration. All setter methods return the locator instance for chaining.

### SetTimeout

Sets the timeout in milliseconds. Default is 30000 (30 seconds).

```cs
await page.Locator("button.submit")
    .SetTimeout(5000)
    .ClickAsync();
```

### SetVisibility

Sets whether to wait for the element to be visible or hidden.

```cs
// Wait for element to be visible before clicking
await page.Locator("button.submit")
    .SetVisibility(VisibilityOption.Visible)
    .ClickAsync();

// Wait for a loading spinner to disappear
await page.Locator(".spinner")
    .SetVisibility(VisibilityOption.Hidden)
    .WaitAsync();
```

### SetWaitForEnabled

Sets whether to wait for the element to be enabled before acting. Default is `true`.

```cs
await page.Locator("button.submit")
    .SetWaitForEnabled(true)
    .ClickAsync();
```

### SetEnsureElementIsInTheViewport

Sets whether to automatically scroll the element into the viewport. Default is `true`.

```cs
await page.Locator("button.submit")
    .SetEnsureElementIsInTheViewport(true)
    .ClickAsync();
```

### SetWaitForStableBoundingBox

Sets whether to wait for the element's bounding box to stabilize (stop moving) before acting. Default is `true`.

```cs
await page.Locator("button.submit")
    .SetWaitForStableBoundingBox(true)
    .ClickAsync();
```

### Combining Options

```cs
await page.Locator("button.submit")
    .SetTimeout(10000)
    .SetVisibility(VisibilityOption.Visible)
    .SetWaitForEnabled(true)
    .SetEnsureElementIsInTheViewport(true)
    .SetWaitForStableBoundingBox(true)
    .ClickAsync();
```

## Actions

### ClickAsync

Clicks the located element.

```cs
await page.Locator("button.submit").ClickAsync();
```

See [Locator Actions](LocatorActions.md) for click options (button, count, delay, offset).

### HoverAsync

Hovers over the located element.

```cs
await page.Locator("button.submit").HoverAsync();
```

### FillAsync

Fills the located element with a value. Works with `<input>`, `<textarea>`, `<select>`, and `contenteditable` elements.

```cs
// Fill a text input
await page.Locator("input[name='email']").FillAsync("user@example.com");

// Select an option
await page.Locator("select#country").FillAsync("US");
```

### ScrollAsync

Scrolls the located element.

```cs
await page.Locator("div.scrollable").ScrollAsync(new LocatorScrollOptions
{
    ScrollTop = 500
});
```

See [Locator Actions](LocatorActions.md) for scroll options.

## Waiting

### WaitAsync

Waits for the locator to locate an element matching the configured preconditions.

```cs
// Wait for an element to appear
await page.Locator("div.loaded").WaitAsync();
```

### WaitAsync&lt;T&gt;

Waits for the locator to get a value from the page, returned as a JSON-deserialized object.

```cs
var value = await page.LocatorFunction("() => document.title")
    .WaitAsync<string>();
```

### WaitHandleAsync

Waits for the locator to get a handle from the page.

```cs
var handle = await page.Locator("button.submit").WaitHandleAsync();
```

## Composition

### Filter

Creates a new locator that filters elements using a JavaScript predicate. If the predicate returns false, the locator retries.

```cs
await page.Locator("button")
    .Filter("element => element.textContent === 'Submit'")
    .ClickAsync();
```

### Map

Creates a new locator that maps the located element using a JavaScript function.

```cs
var text = await page.Locator("div.content")
    .Map("element => element.textContent")
    .WaitAsync<string>();
```

### Locator.Race

Races multiple locators and resolves with the first one to find an element.

```cs
await Locator.Race(
    page.Locator("button.accept"),
    page.Locator("button.dismiss")
).ClickAsync();
```

## CancellationToken Support

All locator actions accept a `CancellationToken` through their options parameter:

```cs
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

await page.Locator("button.submit").ClickAsync(new LocatorClickOptions
{
    CancellationToken = cts.Token
});
```

```cs
await page.Locator("div.loaded").WaitAsync(new LocatorActionOptions
{
    CancellationToken = cts.Token
});
```
