# How to map a javascript object to a .NET object

_Contributors: [Bilal Durrani](https://bilaldurrani.io/)_

## Problem

You need to map javascript object to a .NET object.

## Solution

Use `Page.EvaluateFunctionAsync<T>` to evaluate a javascript function in the context of the browser
and return a .NET object of type `T`.

```cs

public class Data
{
    public string Title { get; set; }
    public string Url { get; set; }
} 

using (var browser = await Puppeteer.LaunchAsync(options))
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("https://news.ycombinator.com/");
    Console.WriteLine("Get all urls from page");
    var jsCode = @"() => {
const selectors = Array.from(document.querySelectorAll('a[class=""storylink""]')); 
return selectors.map( t=> {return { title: t.innerHTML, url: t.href}});
}";
    var results = await page.EvaluateFunctionAsync<Data[]>(jsCode);
    foreach (var result in results)
    {
        Console.WriteLine(result.ToString());
    }
    Console.WriteLine("Press any key to continue...");
    Console.ReadLine();
} 
```
