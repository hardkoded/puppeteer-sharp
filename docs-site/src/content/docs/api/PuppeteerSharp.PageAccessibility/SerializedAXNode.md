---
title: "SerializedAXNode"
description: "AXNode."
editUrl: false
prev: false
next: false
sidebar:
  label: "SerializedAXNode"
  hidden: true
---

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode"></a>

Namespace: [PuppeteerSharp.PageAccessibility](/api/PuppeteerSharp.PageAccessibility/)


```csharp
public class SerializedAXNode : IEquatable<SerializedAXNode>
```


AXNode.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;SerializedAXNode&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L10)

## Constructors

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode__ctor_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode__ctor"></a>

### SerializedAXNode()


```csharp
public SerializedAXNode()
```


Initializes a new instance of the [SerializedAXNode](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L15)

## Properties

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Atomic_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Atomic"></a>

### Atomic


```csharp
public bool Atomic { get; set; }
```


Whether the live region is &lt;a href="https://www.w3.org/TR/wai-aria/#aria-atomic"&gt;atomic&lt;/a&gt;.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L155)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_AutoComplete_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_AutoComplete"></a>

### AutoComplete


```csharp
public string AutoComplete { get; set; }
```


What kind of autocomplete is supported by a control.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L125)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Busy_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Busy"></a>

### Busy


```csharp
public bool Busy { get; set; }
```


Whether the node is &lt;a href="https://www.w3.org/TR/wai-aria/#aria-busy"&gt;busy&lt;/a&gt;.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L145)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Checked_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Checked"></a>

### Checked


```csharp
public CheckedState Checked { get; set; }
```


Whether the checkbox is checked, or "mixed".

**Value:** [CheckedState](/api/PuppeteerSharp.PageAccessibility/CheckedState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L100)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Children_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Children"></a>

### Children


```csharp
public SerializedAXNode[] Children { get; set; }
```


Child nodes of this node, if any.

**Value:** `SerializedAXNode\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L185)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Description_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Description"></a>

### Description


```csharp
public string Description { get; set; }
```


An additional human readable description of the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L35)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Details_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Details"></a>

### Details


```csharp
public string Details { get; set; }
```


The &lt;a href="https://www.w3.org/TR/wai-aria/#aria-details"&gt;details&lt;/a&gt; for the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L170)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Disabled_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Disabled"></a>

### Disabled


```csharp
public bool Disabled { get; set; }
```


Whether the node is disabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L55)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Errormessage_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Errormessage"></a>

### Errormessage


```csharp
public string Errormessage { get; set; }
```


The &lt;a href="https://www.w3.org/TR/wai-aria/#aria-errormessage"&gt;error message&lt;/a&gt; for the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L165)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Expanded_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Expanded"></a>

### Expanded


```csharp
public bool Expanded { get; set; }
```


Whether the node is expanded or collapsed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L60)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Focused_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Focused"></a>

### Focused


```csharp
public bool Focused { get; set; }
```


Whether the node is focused.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L65)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_HasPopup_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_HasPopup"></a>

### HasPopup


```csharp
public string HasPopup { get; set; }
```


What kind of popup is currently being shown for a node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L130)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Invalid_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Invalid"></a>

### Invalid


```csharp
public string Invalid { get; set; }
```


Whether and in what way this node's value is invalid.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L135)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_KeyShortcuts_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_KeyShortcuts"></a>

### KeyShortcuts


```csharp
public string KeyShortcuts { get; set; }
```


Keyboard shortcuts associated with this node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L40)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Level_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Level"></a>

### Level


```csharp
public int Level { get; set; }
```


The level of a heading.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L110)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Live_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Live"></a>

### Live


```csharp
public string Live { get; set; }
```


The &lt;a href="https://www.w3.org/TR/wai-aria/#aria-live"&gt;live&lt;/a&gt; status of the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L150)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_LoaderId_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_LoaderId"></a>

### LoaderId


```csharp
public string LoaderId { get; set; }
```


CDP-specific documentId. LoaderId is an experimental mechanism to establish unique IDs across navigations.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L180)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Modal_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Modal"></a>

### Modal


```csharp
public bool Modal { get; set; }
```


Whether the node is &lt;a href="https://en.wikipedia.org/wiki/Modal_window"&gt;modal&lt;/a&gt;.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L70)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Multiline_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Multiline"></a>

### Multiline


```csharp
public bool Multiline { get; set; }
```


Whether the node text input supports multiline.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L75)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Multiselectable_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Multiselectable"></a>

### Multiselectable


```csharp
public bool Multiselectable { get; set; }
```


Whether more than one child can be selected.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L80)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Name_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Name"></a>

### Name


```csharp
public string Name { get; set; }
```


A human readable name for the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L25)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Orientation_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Orientation"></a>

### Orientation


```csharp
public string Orientation { get; set; }
```


Whether the node is oriented horizontally or vertically.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L140)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Pressed_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Pressed"></a>

### Pressed


```csharp
public CheckedState Pressed { get; set; }
```


Whether the toggle button is checked, or "mixed".

**Value:** [CheckedState](/api/PuppeteerSharp.PageAccessibility/CheckedState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L105)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Readonly_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Readonly"></a>

### Readonly


```csharp
public bool Readonly { get; set; }
```


Whether the node is read only.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L85)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Relevant_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Relevant"></a>

### Relevant


```csharp
public string Relevant { get; set; }
```


The &lt;a href="https://www.w3.org/TR/wai-aria/#aria-relevant"&gt;relevant&lt;/a&gt; changes for the live region.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L160)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Required_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Required"></a>

### Required


```csharp
public bool Required { get; set; }
```


Whether the node is required.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L90)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Role_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Role"></a>

### Role


```csharp
public string Role { get; set; }
```


The &lt;see fref="https://www.w3.org/TR/wai-aria/#usage_intro"&gt;role&lt;/see&gt;.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L20)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_RoleDescription_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_RoleDescription"></a>

### RoleDescription


```csharp
public string RoleDescription { get; set; }
```


A human readable alternative to the role.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L45)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Selected_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Selected"></a>

### Selected


```csharp
public bool Selected { get; set; }
```


Whether the node is selected in its parent node.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L95)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Url_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


URL for link elements.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L175)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Value_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Value"></a>

### Value


```csharp
public string Value { get; set; }
```


The current value of the node.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L30)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueMax_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueMax"></a>

### ValueMax


```csharp
public int ValueMax { get; set; }
```


The maximum value in a node.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L120)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueMin_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueMin"></a>

### ValueMin


```csharp
public int ValueMin { get; set; }
```


The minimum value in a node.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L115)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueText_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ValueText"></a>

### ValueText


```csharp
public string ValueText { get; set; }
```


A description of the current value.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L50)

## Methods

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ElementHandleAsync_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_ElementHandleAsync"></a>

### ElementHandleAsync()


```csharp
public Task<IElementHandle> ElementHandleAsync()
```


Gets the element handle for this node.
For text nodes, returns the parent element's handle instead of the text node's.
Returns `null` if the frame has navigated to a new document since the snapshot.

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — An [IElementHandle](/api/PuppeteerSharp/IElementHandle/) for this node, or `null` if no handle is available.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L195)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Equals_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Equals_PuppeteerSharp_PageAccessibility_SerializedAXNode_"></a>

### Equals(SerializedAXNode)


```csharp
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Exceptions should not be raised in this type of method.")]
public bool Equals(SerializedAXNode other)
```


Indicates whether the current object is equal to another object of the same type.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `other` | [SerializedAXNode](./) | An object to compare with this object. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;true&lt;/a&gt; if the current object is equal to the &lt;code class="paramref"&gt;other&lt;/code&gt; parameter; otherwise, &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;false&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L199)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_Equals_System_Object_"></a>

### Equals(object)


```csharp
public override bool Equals(object obj)
```


Determines whether the specified object is equal to the current object.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `obj` | [object](https://learn.microsoft.com/dotnet/api/system.object) | The object to compare with the current object. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;true&lt;/a&gt; if the specified object  is equal to the current object; otherwise, &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;false&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L236)

<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_GetHashCode_"></a>
<a id="PuppeteerSharp_PageAccessibility_SerializedAXNode_GetHashCode"></a>

### GetHashCode()


```csharp
public override int GetHashCode()
```


Serves as the default hash function.

**Returns:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — A hash code for the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/SerializedAXNode.cs#L239)
