# Locator Actions

This guide covers the action methods available on locators and their option classes.

## ClickAsync

Clicks the located element. The locator waits for the element to pass all configured preconditions before clicking.

```cs
await page.Locator("button.submit").ClickAsync();
```

### LocatorClickOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Button` | `MouseButton` | `MouseButton.Left` | Mouse button to use. |
| `Count` | `int` | `1` | Number of clicks (e.g., 2 for double-click). |
| `Delay` | `int` | `0` | Time in milliseconds between mousedown and mouseup. |
| `OffSet` | `Offset?` | `null` | Click offset relative to the element's padding box. |
| `CancellationToken` | `CancellationToken` | `default` | Token to cancel the action. |

### Examples

**Double-click:**

```cs
await page.Locator("div.editable").ClickAsync(new LocatorClickOptions
{
    Count = 2
});
```

**Right-click:**

```cs
await page.Locator("div.context-menu-target").ClickAsync(new LocatorClickOptions
{
    Button = MouseButton.Right
});
```

**Click with offset:**

```cs
await page.Locator("canvas").ClickAsync(new LocatorClickOptions
{
    OffSet = new Offset { X = 100, Y = 50 }
});
```

**Slow click (with delay):**

```cs
await page.Locator("button.submit").ClickAsync(new LocatorClickOptions
{
    Delay = 100
});
```

## HoverAsync

Hovers over the located element. Useful for triggering hover states, tooltips, or dropdown menus.

```cs
await page.Locator("button.submit").HoverAsync();
```

### LocatorActionOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CancellationToken` | `CancellationToken` | `default` | Token to cancel the action. |

## FillAsync

Fills the located element with a value. Automatically detects the element type and handles it appropriately:

- **`<input>`**: Focuses, clears, and types the value.
- **`<textarea>`**: Focuses, clears, and types the value.
- **`<select>`**: Selects the option with the matching value.
- **`contenteditable`**: Focuses, clears, and types the value.

```cs
await page.Locator("input[name='email']").FillAsync("user@example.com");
```

### LocatorActionOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CancellationToken` | `CancellationToken` | `default` | Token to cancel the action. |

## ScrollAsync

Scrolls the located element to a specific position.

```cs
await page.Locator("div.scrollable").ScrollAsync(new LocatorScrollOptions
{
    ScrollTop = 500,
    ScrollLeft = 0
});
```

### LocatorScrollOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ScrollTop` | `decimal?` | `null` | Vertical scroll position. |
| `ScrollLeft` | `decimal?` | `null` | Horizontal scroll position. |
| `CancellationToken` | `CancellationToken` | `default` | Token to cancel the action. |

## Preconditions

Before every action, the locator checks the following preconditions (when enabled):

1. **Visibility**: Waits for the element to match the configured visibility (`Visible` or `Hidden`).
2. **Enabled**: Waits for the element to be enabled (not disabled).
3. **Viewport**: Scrolls the element into the viewport if needed.
4. **Stable bounding box**: Waits for the element to stop moving (e.g., after animations).

These are all enabled by default and can be configured per-locator:

```cs
await page.Locator("button.animated")
    .SetWaitForStableBoundingBox(true)  // wait for animation to finish
    .SetWaitForEnabled(true)             // wait for button to be enabled
    .SetEnsureElementIsInTheViewport(true) // scroll into view
    .ClickAsync();
```

## Retry Behavior

If an action fails due to a precondition not being met, the locator retries automatically with a 100ms delay between attempts, until the configured timeout is reached (default: 30 seconds).
