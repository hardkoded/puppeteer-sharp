---
title: "SecurityDetails"
description: "Represents the security details when response was received over the secure connection."
editUrl: false
prev: false
next: false
sidebar:
  label: "SecurityDetails"
  hidden: true
---

<a id="PuppeteerSharp_SecurityDetails"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class SecurityDetails
```


Represents the security details when response was received over the secure connection.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L7)

## Constructors

<a id="PuppeteerSharp_SecurityDetails__ctor_"></a>
<a id="PuppeteerSharp_SecurityDetails__ctor"></a>

### SecurityDetails()


```csharp
public SecurityDetails()
```


Initializes a new instance of the [SecurityDetails](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L12)

<a id="PuppeteerSharp_SecurityDetails__ctor_System_String_System_String_System_Int64_System_Int64_System_String_"></a>

### SecurityDetails(string, string, long, long, string)


```csharp
public SecurityDetails(string subjectName, string issuer, long validFrom, long validTo, string protocol)
```


Initializes a new instance of the [SecurityDetails](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `subjectName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Subject name. |
| `issuer` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Issuer. |
| `validFrom` | [long](https://learn.microsoft.com/dotnet/api/system.int64) | Valid from. |
| `validTo` | [long](https://learn.microsoft.com/dotnet/api/system.int64) | Valid to. |
| `protocol` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Protocol. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L24)

<a id="PuppeteerSharp_SecurityDetails__ctor_System_String_System_String_System_Int64_System_Int64_System_String_System_String___"></a>

### SecurityDetails(string, string, long, long, string, string[])


```csharp
public SecurityDetails(string subjectName, string issuer, long validFrom, long validTo, string protocol, string[] subjectAlternativeNames)
```


Initializes a new instance of the [SecurityDetails](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `subjectName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Subject name. |
| `issuer` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Issuer. |
| `validFrom` | [long](https://learn.microsoft.com/dotnet/api/system.int64) | Valid from. |
| `validTo` | [long](https://learn.microsoft.com/dotnet/api/system.int64) | Valid to. |
| `protocol` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Protocol. |
| `subjectAlternativeNames` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Subject alternative names. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L38)

## Properties

<a id="PuppeteerSharp_SecurityDetails_Issuer_"></a>
<a id="PuppeteerSharp_SecurityDetails_Issuer"></a>

### Issuer


```csharp
public string Issuer { get; set; }
```


Gets the issuer.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The issuer.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L58)

<a id="PuppeteerSharp_SecurityDetails_Protocol_"></a>
<a id="PuppeteerSharp_SecurityDetails_Protocol"></a>

### Protocol


```csharp
public string Protocol { get; set; }
```


Gets the protocol.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The protocol.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L76)

<a id="PuppeteerSharp_SecurityDetails_SubjectAlternativeNames_"></a>
<a id="PuppeteerSharp_SecurityDetails_SubjectAlternativeNames"></a>

### SubjectAlternativeNames


```csharp
public string[] SubjectAlternativeNames { get; set; }
```


Gets the list of subject alternative names (SANs) of the certificate.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) — The list of subject alternative names (SANs) of the certificate.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L82)

<a id="PuppeteerSharp_SecurityDetails_SubjectName_"></a>
<a id="PuppeteerSharp_SecurityDetails_SubjectName"></a>

### SubjectName


```csharp
public string SubjectName { get; set; }
```


Gets the name of the subject.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The name of the subject.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L52)

<a id="PuppeteerSharp_SecurityDetails_ValidFrom_"></a>
<a id="PuppeteerSharp_SecurityDetails_ValidFrom"></a>

### ValidFrom


```csharp
public long ValidFrom { get; set; }
```


Gets the valid from.

**Value:** [long](https://learn.microsoft.com/dotnet/api/system.int64) — The valid from.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L64)

<a id="PuppeteerSharp_SecurityDetails_ValidTo_"></a>
<a id="PuppeteerSharp_SecurityDetails_ValidTo"></a>

### ValidTo


```csharp
public long ValidTo { get; set; }
```


Gets the valid to.

**Value:** [long](https://learn.microsoft.com/dotnet/api/system.int64) — The valid to.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SecurityDetails.cs#L70)
