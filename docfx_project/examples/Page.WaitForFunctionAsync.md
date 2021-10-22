# How to wait for a javascript expression to be true
_Contributors: [Darío Kondratiuk](https://www.hardkoded.com)_

## Problem

`WaitForSelectorAsync` is not enough, you want to wait for a more complex javascript expression to be truthly.

## Solution

Use [Page.WaitForExpressionAsync](https://www.puppeteersharp.com/api/PuppeteerSharp.Page.html#PuppeteerSharp_Page_WaitForExpressionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_) or [Page.WaitForFunctionAsync](https://www.puppeteersharp.com/api/PuppeteerSharp.Page.html#PuppeteerSharp_Page_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___) to delay execution until the result of a javascription expression is truthly.

If it's a simple expression you can use `WaitForFunctionAsync`:

```cs
using (var browser = await Puppeteer.LaunchAsync(options))
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("https://www.somepage.com");
    await Page.WaitForExpressionAsync("document.queryselector('#status_info').innerText.match('^Showing ([1-9][0-9]*?) to ([1-9][0-9]*?)') of ([1-9][0-9]*?) entries') != null");
}
```

If the evaluation is more complex, you could wrap it inside a function and use `WaitForFunctionAsync`:

```cs
var waitTask = Page.WaitForFunctionAsync(@"() =>
{
    return document.queryselector('#status_info').innerText.match('^Showing ([1-9][0-9]*?) to ([1-9][0-9]*?)') of ([1-9][0-9]*?) entries') != null;
}");
```
