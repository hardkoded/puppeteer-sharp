---
title: "DeleteCookiesRequest"
description: "Request to delete cookies matching specific filters."
editUrl: false
prev: false
next: false
sidebar:
  label: "DeleteCookiesRequest"
  hidden: true
---

<a id="PuppeteerSharp_DeleteCookiesRequest"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class DeleteCookiesRequest
```


Request to delete cookies matching specific filters.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L6)

## Properties

<a id="PuppeteerSharp_DeleteCookiesRequest_Domain_"></a>
<a id="PuppeteerSharp_DeleteCookiesRequest_Domain"></a>

### Domain


```csharp
public string Domain { get; set; }
```


Gets or sets the domain. If specified, deletes only cookies with the exact domain.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L22)

<a id="PuppeteerSharp_DeleteCookiesRequest_Name_"></a>
<a id="PuppeteerSharp_DeleteCookiesRequest_Name"></a>

### Name


```csharp
public string Name { get; set; }
```


Gets or sets the name of the cookies to remove.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L11)

<a id="PuppeteerSharp_DeleteCookiesRequest_PartitionKey_"></a>
<a id="PuppeteerSharp_DeleteCookiesRequest_PartitionKey"></a>

### PartitionKey


```csharp
public CookiePartitionKey PartitionKey { get; set; }
```


Gets or sets the partition key. If specified, deletes cookies in the given partition key.
In Chrome, partitionKey matches the top-level site the partitioned cookie is available in.
In Firefox, it matches the source origin.

**Value:** [CookiePartitionKey](/api/PuppeteerSharp/CookiePartitionKey/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L34)

<a id="PuppeteerSharp_DeleteCookiesRequest_Path_"></a>
<a id="PuppeteerSharp_DeleteCookiesRequest_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


Gets or sets the path. If specified, deletes only cookies with the exact path.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L27)

<a id="PuppeteerSharp_DeleteCookiesRequest_Url_"></a>
<a id="PuppeteerSharp_DeleteCookiesRequest_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets or sets the URL. If specified, deletes all the cookies with the given name where domain and path match
the provided URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeleteCookiesRequest.cs#L17)
