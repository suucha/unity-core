# Unity核心类库 - 埋点API

## 概述

埋点API是Unity核心类库的一部分，用于收集用户行为数据，以便我们更好地理解用户的使用习惯和需求，从而优化我们的产品和服务。通过埋点，我们可以了解用户的使用路径、热门功能，改进体验和功能，提升用户满意度和留存率，实现数据驱动决策。

## 埋点数据

埋点数据主要包括两部分：事件名称和事件参数。

### 事件名称

事件名称是用来标识用户行为的，例如"点击购买按钮"、"完成订单"等。

### 事件参数

事件参数用来描述用户行为和上下文，包括公共参数和特有参数。

#### 公共参数

公共参数是每个事件都需要的，如用户标识和设备信息等。其中，一些是静态的数据，比如用户标识；而有一些是动态的数据，比如每次上报事件时都想知道用户当前的资产情况，这个资产是实时变化的。
> 公共参数因为

#### 特有参数

特有参数是针对各个事件的，例如订单事件中的订单金额就是其特有的参数。

## API使用

我们提供的API能帮助你实现上述情况。具体使用方法和示例代码，请参见下文。
### 第三方服务
虽然我们的核心库并未提供自己的事件上报器，但是可以很方便地集成第三方的服务。下面以Google Firebase Analytics为例给出集成方法：

首先，新建一个类，继承自抽象类LogEventReporterAbstract，然后实现以下方法：

```csharp
public class FirebaseAnalyticsLogEventReporter : LogEventReporterAbstract
{
    public override string Name => "FirebaseAnalytics";

    protected override Task LogEventInternal(string name, Dictionary<string, string> eventParameters)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(name, ToParameters(eventParameters));
        return Task.CompletedTask;
    }
    private Parameter[] ToParameters(Dictionary<string, string> eventParameters)
    {
        var parameters = new List<Parameter>();
        foreach(var key in eventParameters.Keys)
        {
            parameters.Add(new Parameter(key, eventParameters[key]));
        }
        return parameters.ToArray();
    }
}
```

然后，使用下面的语句添加一个FirebaseAnalyticsLogEventReporter的实例：

```csharp
Suucha.App.AddEventReporter(new FirebaseAnalyticsLogEventReporter());
```

这样，只要通过Suucha.App.LogEvent方法上报的事件都会上报到Google Firebase。
> Suucha Unity Core支持同时添加多个第三方埋点服务，只需要初始化不同的上报器实例，然后用Suucha.App.AddEventReporter方法添加这个实例就可以了。

> 一般情况下，我们会用扩展方法来简化使用，比如：
> ```csharp
>        public static FirebaseAnalyticsLogEventReporter InitFirebaseAnalyticsLogEvent(this Suucha suucha,
>            List<string> allowedEventNames = null,
>            List<string> excludedEventNames = null,
>            Dictionary<string, string> eventNameMap = null,
>            Dictionary<string, string> eventParameterNameMap = null)
>        {
>            if (!firebaseAnalyticsLogEventAdded)
>            {
>                reporter = new FirebaseAnalyticsLogEventReporter(allowedEventNames,
>                    excludedEventNames, 
>                    eventNameMap,
>                    eventParameterNameMap);
>                firebaseAnalyticsLogEventAdded = true;
>            }
>            return reporter;
>        }
> ```
> 同时，针对ILogEventReporter也有扩展方法：
> ``` csharp
>         public static void Use<TLogEventReporter>(this TLogEventReporter logEventReporter) 
>            where TLogEventReporter : ILogEventReporter
>        {
>            Suucha.App.AddLogEventReporter(logEventReporter);
>        }
> ```
> 这样就可以简化使用如下：
> ``` csharp 
> Suucha.App.InitFirebaseAnalyticsLogEvent().Use();
> ```
### 设置静态公共事件参数

我们提供了一个API，用于设置静态公共事件参数。这个API的使用方法如下：

```csharp
reporter.AddCommonEventParameter("parameterName", "value");
```

在这个API中，"parameterName"是你要设置的参数的名称，"value"是你要设置的参数的值。

例如，如果你想设置用户标识为"123456"，你可以这样使用这个API：

```csharp
reporter.AddCommonEventParameter("userId", "123456");
```
> 因为每个第三方服务可能需要的公共参数不同，所以需要分别对不同的第三方Reporter的公共参数需要设置。比如，针对设备信息，大多第三方会自己收集设备信息上报，不需要设置设备信息类的公共参数；像UserId这样的，大多第三方也提供了自己的API来设置如AppsFlyer的SetCustomerUserId。

> 但是为了降低复杂性，通过事件参数拦截器产生的公共参数却是针对所有第三方Reporter的。
### 设置动态公共事件参数

设置动态公共事件参数需要使用拦截器，这部分内容将在后面的文档中介绍。

### 上报埋点事件

我们提供了两个API，用于上报埋点事件。

#### 上报不带任何事件参数的事件

如果你想上报一个不带任何事件参数的事件，你可以使用以下API：

```csharp
Suucha.App.LogEvent("event_name");
```

在这个API中，"event_name"是你要上报的事件的名称。

例如，如果你想上报一个名为"click_buy_button"的事件，你可以这样使用这个API：

```csharp
Suucha.App.LogEvent("click_buy_button");
```

#### 上报带有特定事件参数的事件

如果你想上报一个带有特定事件参数的事件，你可以使用以下API：

```csharp
Suucha.App.LogEvent("event_name", new Dictionary<string, string>{{"parameterName1", "value1"},{"paramenterName2", "value2"}});
```

在这个API中，"event_name"是你要上报的事件的名称，后面的字典是你要上报的事件参数。

例如，如果你想上报一个名为"complete_order"的事件，并且这个事件有两个参数："order_amount"和"order_id"，你可以这样使用这个API：

```csharp
Suucha.App.LogEvent("complete_order", new Dictionary<string, string>{{"order_amount", "100"},{"order_id", "123456"}});
```
> 这个字典里面的事件参数名称如果和公共事件参数名称同名，就会用这个值覆盖公共事件参数中的值。
## 拦截器

我们提供了两种类型的拦截器，一种是事件拦截器，它可以产生新的事件和增加事件参数；另一种是事件参数拦截器，它只能增加或修改事件参数。

### 事件拦截器

事件拦截器的接口定义如下：

```csharp
/// <summary>
/// 事件拦截器接口。
/// </summary>
public interface ILogEventIntercept
{
    /// <summary>
    /// 获取拦截器的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取拦截器的执行顺序。顺序值越小的拦截器会先执行。
    /// </summary>
    int Order { get; }

    /// <summary>
    /// 执行拦截器。
    /// </summary>
    /// <param name="eventName">事件名称。</param>
    /// <param name="parameters">事件参数。</param>
    /// <returns>表示异步操作的任务。任务的结果包含一个字典，字典的键是事件名称，值是该事件的参数。</returns>
    UniTask<Dictionary<string, Dictionary<string, string>>> Execute(string eventName, Dictionary<string, string> parameters);
}
```
通过扩展方法Suucha.App.AddLogEventIntercept()添加一个事件拦截器实例，这个实例是全局的，并不是某个第三方上报器特有的，但是默认情况下，第三方上报器也没有添加这个拦截器，通过
``` csharp 
reporter.EnableEventIntercept<TLogEventIntercept>();
```
方法为第三方上报器添加这个拦截器。
> 如果拦截器没有通过Suucha.App.AddLogEventIntercept添加，就算调用了EnableEventIntercept方法，此拦截器也不会生效。
### 事件参数拦截器

事件参数拦截器的接口定义如下：

```csharp
/// <summary>
/// 事件参数拦截器接口。
/// </summary>
public interface ILogEventParameterIntercept
{
    /// <summary>
    /// 获取拦截器的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取拦截器的执行顺序。顺序值越小的拦截器会先执行。
    /// </summary>
    int Order { get; }

    /// <summary>
    /// 执行拦截器。
    /// </summary>
    /// <param name="eventName">事件名称。</param>
    /// <param name="parameters">事件参数。</param>
    /// <returns>表示异步操作的任务。任务的结果包含一个字典，字典的键是参数名称，值是参数值。</returns>
    UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters);
}
```
通过扩展方法Suucha.App.AddLogEventParameterIntercept来添加事件参数拦截器。
> 为了减少复杂性，事件参数拦截器是全局的，只要添加了，所有第三方上报器上报事件的时候都会增加此参数拦截器新增或修改的事件参数。
### 内置拦截器

我们的核心类库中内置了一些拦截器，下面我将分别介绍：

#### App运行时长拦截器

这是一个事件参数拦截器，在每一个上报的事件中添加上报事件时app运行了多长时间，此拦截器产生的事件参数名称为"app_run_duration"，此参数的单位是毫秒，可以帮助开发者分析app耗时情况。

使用这个拦截器的示例代码如下：

```csharp
Suucha.App.EnableRunDurationEventParameter();
```

#### 事件名称计数拦截器

此拦截器为事件拦截器，作用是根据开发者的配置，可以指定某些事件，在设定的次数到达时生成一个新的事件，名称格式为：{eventName}_{count}。比如要求在spin 10次的时候上报一个"spin_10"的事件。

事件名称计数配置类定义如下：

```csharp
public class EventCounterConfiguration
{
    public EventCounterConfiguration()
    {
        CountList = new List<int>();
    }
    /// <summary>
    /// 需要计数的事件名称，
    /// 如果名称以_结束，则表示所有以此开始的事件都需要计数
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }
    /// <summary>
    /// 需要计数个数的列表
    /// </summary>
    [JsonProperty("countList")]
    public List<int> CountList { get; set; }
}
```

使用这个拦截器的示例代码如下：

```csharp
Suucha.App.AddEventCounter(new List<EventCounterConfiguration>{new EventCounterConfiguration{Name="spin", CountList=new List<int>{1,5,10,100}} });
```
第三方上报器启用此拦截器：
``` csharp
reporter.EnableEventCounter();
```
这样配置下，当记录spin分别为1，5，10，100次时，会同时上报spin_001, spin_005, spin_010, spin_100这几个事件，其事件参数同同时上报的spin事件的事件参数一样，名称中的次数会根据CountList列表中最大的次数自动补0。

#### 其他事件拦截器
还有一些事件拦截器，比如和资产相关的、同广告相关的会在相应的模块文档中介绍。

### 事件名称映射和事件参数名称映射

由于我们的核心库提供了统一的API来上报事件，所以事件名称和事件参数名称可能不能完全满足第三方服务的规则。例如，对于购买事件，Appsflyer建议的名称是"af_purchase"，而Google建议的名称是"in_app_purchase"。

为了解决这个问题，我们在初始化第三方事件上报器的时候可以提供事件名称映射列表和事件参数名称映射列表。这样，即使你的事件名称和参数名称与第三方服务的规则不完全匹配，也可以在第三方的管理台上直观地看到统计和分析数据。

当然，你也可以选择不提供映射列表，但这可能会影响到你在第三方管理台上查看数据的便利性。

### 事件参数的重要性

在某些情况下，如果事件缺失某些参数，可能会导致统计分析的准确性降低。例如，对于app第一次运行的事件，如果这时候事件参数缺失归因数据、渠道数据、AB分组等数据，对于用户的安装来源、AB分组等维度的分析准确性会降低。

因此，在初始化第三方事件上报器时，你可以选择设置一些公共的事件参数。如果这些参数没有设置，事件将被缓存在本地，而不是立即上报给第三方服务。等到这些公共事件参数全部设置了，再上报给第三方服务。这样可以确保你的统计分析数据的准确性。
可以用如下扩展方法添加必须的事件参数：
``` csharp
//方法定义：
//requiredParameters是必须的参数，其中key为参数名称，value为默认值
//timeout，超时时间，单位毫秒。如果在timeout后还有事件参数为设置，那么就用提供的默认值上报事件
AddRequiredEventParameters(Dictionary<string, string> requiredParameters,
int timeout)
//使用示例，如果timeout指定的时间后还没有提供media_source，那么用organic上报
Suucha.App.InitFirebaseAnalyticsLogEvent().AddRequiredEventParameters(new Dictionary<string, string>{{"media_soure","organic"}}, 30000).Use();
```
> 注意，Use的调用最好在AddRequiredEventParameters方法之后

## 第三方上报器实现
### AppsFlyer
Github： [AppsFlyer](https://github.com/suucha/unity-appsflyer)
AppsFlyer是一种移动应用分析工具，帮助开发者和营销团队追踪移动应用的用户行为和广告活动的效果。
接入AppsFlyer后，可以用以下代码初始化AppsFlyer和启用AppsFlyer事件上报器：
``` csharp
Suucha.App.InitAppsFlyer("your appsflyer devid").InitAppsFlyerLogEvent().Use();
```

### Google Firebase Analytics
Github: [Google Firebase Analytics](https://github.com/suucha/unity-firebase-analytics)
Google Firebase Analytics是Google Firebase移动应用开发平台的一部分，它为开发者提供了强大的移动应用分析工具，帮助他们更好地了解用户行为、应用性能和用户互动。
接入Firebase Analytics后，可以用以下代码初始化Firebase和启用Firebase Analytics事件上报器：
``` csharp
Suucha.App.InitFirebase().InitFirebaseAnalyticsLogEvent().Use();
```

### 数数
Github: [ThinkingData Analytics](https://github.com/suucha/unity-thinkingdata-analytics)
ThinkingData Analytics为企业提供了一套用于分析和理解用户行为、优化用户体验以及改进市场营销策略的工具和平台。
接入ThinkingData Analytics后，可以用以下代码初始化
``` csharp
Suucha.App.InitThinkingAnalytics("your app id", "your server").InitThinkingAnalyticsReporter().Use();
```

## 参考文章

https://support.appsflyer.com/hc/zh-cn/articles/4410481112081