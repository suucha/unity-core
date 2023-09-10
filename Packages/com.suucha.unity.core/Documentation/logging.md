# Logging
在代码中输出日志而不需要考虑日志输出的实现方式，同时能在日志输出中打印出当前的类名及日志等级。

## LogLevel
日志输出等级有如下几种：
``` csharp
public enum LogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical,
    None
}
```
在代码中使用不同的方法输出不同等级的日志：
``` csharp
public class TestClass : SuuchaBase //SuuchaMonoBehaviourBase
{
    public void TestMethod()
    {
        Logger.LogTrace("Trace message.");
        Logger.LogDebug("Debug message with format, {0}.", DateTime.Now);
        Logger.LogInformation("Information message.");
        Logger.LogWarning("Warning message.");
        Logger.LogError("Error message.");
        Logger.LogCritical("Critical message.");
        Logger.Log(LogLevel.Debug, "Log debug message.");
    }
}
```
## 设置日志输出等级
通过调用Suucha.App的SetLogLevel方法来设置日志输出等级：
``` csharp
Suucha.App.SetLogLevel(LogLevel.Debug);
```
## UnityLogger
Suucha Unity Core实现了一个使用Unity内置的日志输出的实现，通过以下方式使用Unity Logger：
``` csharp
Suucha.App.UseUnityLogger();
```