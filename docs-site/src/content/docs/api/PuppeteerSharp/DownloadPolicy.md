---
title: "DownloadPolicy"
description: "Download policy for controlling download behavior."
editUrl: false
prev: false
next: false
sidebar:
  label: "DownloadPolicy"
  hidden: true
---

<a id="PuppeteerSharp_DownloadPolicy"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<DownloadPolicy>))]
public enum DownloadPolicy
```


Download policy for controlling download behavior.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadPolicy.cs#L10)

## Fields

<a id="PuppeteerSharp_DownloadPolicy_Allow"></a>

### Allow


```csharp
[EnumMember(Value = "allow")]
Allow = 1
```


Allow all download requests.

**Value:** [DownloadPolicy](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadPolicy.cs#L22)

<a id="PuppeteerSharp_DownloadPolicy_AllowAndName"></a>

### AllowAndName


```csharp
[EnumMember(Value = "allowAndName")]
AllowAndName = 2
```


Allow downloads and name files according to their download GUIDs.

**Value:** [DownloadPolicy](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadPolicy.cs#L28)

<a id="PuppeteerSharp_DownloadPolicy_Default"></a>

### Default


```csharp
[EnumMember(Value = "default")]
Default = 3
```


Use default behavior if available.

**Value:** [DownloadPolicy](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadPolicy.cs#L34)

<a id="PuppeteerSharp_DownloadPolicy_Deny"></a>

### Deny


```csharp
[EnumMember(Value = "deny")]
Deny = 0
```


Deny all download requests.

**Value:** [DownloadPolicy](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadPolicy.cs#L16)
