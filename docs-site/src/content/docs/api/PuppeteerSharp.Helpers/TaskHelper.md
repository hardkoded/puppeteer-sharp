---
title: "TaskHelper"
description: "Task helper."
editUrl: false
prev: false
next: false
sidebar:
  label: "TaskHelper"
  hidden: true
---

<a id="PuppeteerSharp_Helpers_TaskHelper"></a>

Namespace: [PuppeteerSharp.Helpers](/api/PuppeteerSharp.Helpers/)


```csharp
public static class TaskHelper
```


Task helper.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L11)

## Properties

<a id="PuppeteerSharp_Helpers_TaskHelper_DefaultTimeout_"></a>
<a id="PuppeteerSharp_Helpers_TaskHelper_DefaultTimeout"></a>

### DefaultTimeout


```csharp
public static int DefaultTimeout { get; set; }
```


Default timeout.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L19)

## Methods

<a id="PuppeteerSharp_Helpers_TaskHelper_WhenAllFailFast_"></a>
<a id="PuppeteerSharp_Helpers_TaskHelper_WhenAllFailFast_System_Threading_Tasks_Task___"></a>

### WhenAllFailFast(Task[])


```csharp
public static Task WhenAllFailFast(this Task[] tasks)
```


Awaits all tasks, but fails immediately if any task fails (like JavaScript's Promise.all).
Unlike `Task%5b%5d)` which waits for all tasks to complete before throwing,
this method throws immediately when any task fails or is canceled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `tasks` | [Task\[\]](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) | The tasks to await. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when all tasks complete successfully.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L199)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout_"></a>
<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout_System_Threading_Tasks_Task_System_Func_System_Threading_Tasks_Task__System_Int32_System_Threading_CancellationToken_"></a>

### `WithTimeout(Task, Func<Task>, int, CancellationToken)`


```csharp
public static Task WithTimeout(this Task task, Func<Task> timeoutAction, int milliseconds = 1000, CancellationToken cancellationToken = default)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after &lt;code class="paramref"&gt;milliseconds&lt;/code&gt; milliseconds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) | Task to wait for. |
| `timeoutAction` | [Func&lt;Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-1) | Action to be executed on Timeout. |
| `milliseconds` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Milliseconds timeout. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | Cancellation token. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L68)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout_System_Threading_Tasks_Task_System_Func_System_Threading_Tasks_Task__System_TimeSpan_System_Threading_CancellationToken_"></a>

### `WithTimeout(Task, Func<Task>, TimeSpan, CancellationToken)`


```csharp
public static Task WithTimeout(this Task task, Func<Task> timeoutAction, TimeSpan timeout, CancellationToken cancellationToken)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after a given &lt;code class="paramref"&gt;timeout&lt;/code&gt; period.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) | Task to wait for. |
| `timeoutAction` | [Func&lt;Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-1) | Action to be executed on Timeout. |
| `timeout` | [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan) | The timeout period. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | Cancellation token. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L85)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout_System_Threading_Tasks_Task_System_Nullable_System_Int32__System_Func_System_TimeSpan_System_Exception__System_Threading_CancellationToken_"></a>

### `WithTimeout(Task, int?, Func<TimeSpan, Exception>, CancellationToken)`


```csharp
public static Task WithTimeout(this Task task, int? milliseconds = null, Func<TimeSpan, Exception> exceptionFactory = null, CancellationToken cancellationToken = default)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after &lt;code class="paramref"&gt;milliseconds&lt;/code&gt; milliseconds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) | Task to wait for. |
| `milliseconds` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Milliseconds timeout. |
| `exceptionFactory` | [Func&lt;TimeSpan, Exception&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Optional timeout exception factory. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | Cancellation token. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L31)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout_System_Threading_Tasks_Task_System_TimeSpan_System_Func_System_TimeSpan_System_Exception__System_Threading_CancellationToken_"></a>

### `WithTimeout(Task, TimeSpan, Func<TimeSpan, Exception>, CancellationToken)`


```csharp
public static Task WithTimeout(this Task task, TimeSpan timeout, Func<TimeSpan, Exception> exceptionFactory = null, CancellationToken cancellationToken = default)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after a given &lt;code class="paramref"&gt;timeout&lt;/code&gt; period.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) | Task to wait for. |
| `timeout` | [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan) | The timeout period. |
| `exceptionFactory` | [Func&lt;TimeSpan, Exception&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Optional timeout exception factory. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | Cancellation token. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L48)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout__1_System_Threading_Tasks_Task___0__System_Action_System_Int32_"></a>

### `WithTimeout<T>(Task<T>, Action, int)`


```csharp
public static Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, int milliseconds = 1000)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after &lt;code class="paramref"&gt;milliseconds&lt;/code&gt; milliseconds.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | Return type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | Task to wait for. |
| `timeoutAction` | [Action](https://learn.microsoft.com/dotnet/api/system.action) | Action to be executed on Timeout. |
| `milliseconds` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Milliseconds timeout. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L120)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout__1_System_Threading_Tasks_Task___0__System_Action_System_TimeSpan_"></a>

### `WithTimeout<T>(Task<T>, Action, TimeSpan)`


```csharp
public static Task<T> WithTimeout<T>(this Task<T> task, Action timeoutAction, TimeSpan timeout)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after a given &lt;code class="paramref"&gt;timeout&lt;/code&gt; period.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | Return type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | Task to wait for. |
| `timeoutAction` | [Action](https://learn.microsoft.com/dotnet/api/system.action) | Action to be executed on Timeout. |
| `timeout` | [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan) | The timeout period. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L133)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout__1_System_Threading_Tasks_Task___0__System_Nullable_System_Int32__System_Func_System_TimeSpan_System_Exception__"></a>

### `WithTimeout<T>(Task<T>, int?, Func<TimeSpan, Exception>)`


```csharp
public static Task<T> WithTimeout<T>(this Task<T> task, int? milliseconds = null, Func<TimeSpan, Exception> exceptionFactory = null)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after &lt;code class="paramref"&gt;milliseconds&lt;/code&gt; milliseconds.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | Task return type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | Task to wait for. |
| `milliseconds` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Milliseconds timeout. |
| `exceptionFactory` | [Func&lt;TimeSpan, Exception&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Optional timeout exception factory. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L164)

<a id="PuppeteerSharp_Helpers_TaskHelper_WithTimeout__1_System_Threading_Tasks_Task___0__System_TimeSpan_System_Func_System_TimeSpan_System_Exception__"></a>

### `WithTimeout<T>(Task<T>, TimeSpan, Func<TimeSpan, Exception>)`


```csharp
public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, Func<TimeSpan, Exception> exceptionFactory = null)
```


Cancels the &lt;code class="paramref"&gt;task&lt;/code&gt; after a given &lt;code class="paramref"&gt;timeout&lt;/code&gt; period.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | Task return type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `task` | [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | Task to wait for. |
| `timeout` | [TimeSpan](https://learn.microsoft.com/dotnet/api/system.timespan) | The timeout period. |
| `exceptionFactory` | [Func&lt;TimeSpan, Exception&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Optional timeout exception factory. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Helpers/TaskHelper.cs#L177)
