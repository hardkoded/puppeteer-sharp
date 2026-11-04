---
title: "CookieSourceScheme"
description: "Represents the source scheme of the origin that originally set the cookie. A value of \"Unset\" allows protocol clients to emulate legacy cookie scope for the..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CookieSourceScheme"
  hidden: true
---

<a id="PuppeteerSharp_CookieSourceScheme"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<CookieSourceScheme>))]
public enum CookieSourceScheme
```


Represents the source scheme of the origin that originally set the cookie. A value of
"Unset" allows protocol clients to emulate legacy cookie scope for the scheme.
This is a temporary ability and it will be removed in the future.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieSourceScheme.cs#L33)

## Fields

<a id="PuppeteerSharp_CookieSourceScheme_NonSecure"></a>

### NonSecure


```csharp
NonSecure = 1
```


Non-secure.

**Value:** [CookieSourceScheme](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieSourceScheme.cs#L44)

<a id="PuppeteerSharp_CookieSourceScheme_Secure"></a>

### Secure


```csharp
Secure = 2
```


Secure.

**Value:** [CookieSourceScheme](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieSourceScheme.cs#L49)

<a id="PuppeteerSharp_CookieSourceScheme_Unset"></a>

### Unset


```csharp
Unset = 0
```


Unset.

**Value:** [CookieSourceScheme](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieSourceScheme.cs#L39)
