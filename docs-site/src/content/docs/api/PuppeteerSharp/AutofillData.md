---
title: "AutofillData"
description: "Data for autofilling form fields. Provide either CreditCard or Address, but not both."
editUrl: false
prev: false
next: false
sidebar:
  label: "AutofillData"
  hidden: true
---

<a id="PuppeteerSharp_AutofillData"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class AutofillData
```


Data for autofilling form fields.
Provide either [CreditCard](#PuppeteerSharp_AutofillData_CreditCard) or [Address](#PuppeteerSharp_AutofillData_Address), but not both.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AutofillData.cs#L7)

## Properties

<a id="PuppeteerSharp_AutofillData_Address_"></a>
<a id="PuppeteerSharp_AutofillData_Address"></a>

### Address


```csharp
public AutofillAddressData Address { get; set; }
```


Gets or sets the address data.
See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address.

**Value:** [AutofillAddressData](/api/PuppeteerSharp/AutofillAddressData/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AutofillData.cs#L19)

<a id="PuppeteerSharp_AutofillData_CreditCard_"></a>
<a id="PuppeteerSharp_AutofillData_CreditCard"></a>

### CreditCard


```csharp
public CreditCardData CreditCard { get; set; }
```


Gets or sets the credit card data.
See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-CreditCard.

**Value:** [CreditCardData](/api/PuppeteerSharp/CreditCardData/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AutofillData.cs#L13)
