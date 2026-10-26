---
title: "AutofillAddressData"
description: "Address data for autofilling. See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address."
editUrl: false
prev: false
next: false
sidebar:
  label: "AutofillAddressData"
  hidden: true
---

<a id="PuppeteerSharp_AutofillAddressData"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class AutofillAddressData
```


Address data for autofilling.
See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AutofillAddressData.cs#L9)

## Properties

<a id="PuppeteerSharp_AutofillAddressData_Fields_"></a>
<a id="PuppeteerSharp_AutofillAddressData_Fields"></a>

### Fields


```csharp
public List<AutofillAddressFieldEntry> Fields { get; set; }
```


Gets or sets the address fields to fill.
Each entry has a `Name` (see [AutofillAddressField](/api/PuppeteerSharp/AutofillAddressField/)) and a `Value`.

**Value:** [List&lt;AutofillAddressFieldEntry&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.list-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AutofillAddressData.cs#L15)
