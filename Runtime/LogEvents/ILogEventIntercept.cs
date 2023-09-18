using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.LogEvents.Intercepts;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core.LogEvents
{

    public interface ILogEventIntercept
    {
        /// <summary>
        /// Gets the name of the log event interceptor.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the order of the log event interceptor.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Executes the log event interceptor.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="parameters">The parameters of the event.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the modified parameters of the event.</returns>
        UniTask<List<string>> Execute(string eventName, Dictionary<string, string> parameters);

    }
    /*
    /// <summary>
    /// 埋点事件拦截
    /// </summary>
    public interface ILogEventIntercept
    {
        /// <summary>
        /// 拦截器名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 拦截器排序
        /// </summary>
        int Order { get; }
        /// <summary>
        /// 是否是全局拦截器，如果是全局拦截器，那么不允许在事件上报器中添加单独的实例，因为这样可能会导致数据出现混乱。
        /// 比如对于事件计数拦截器，有可能会重复计数。
        /// </summary>
        bool IsGlobal { get; }
        /// <summary>
        /// 执行拦截器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="parameters">事件已有参数</param>
        /// <returns>返回新事件和事件参数。可产生新事件或者仅仅增加事件参数</returns>
        UniTask<Dictionary<string, Dictionary<string, string>>> Execute(string eventName, Dictionary<string, string> parameters);
        /// <summary>
        /// 执行拦截器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="parameters">事件已有参数</param>
        /// <returns>返回新事件和事件参数。可产生新事件或者仅仅增加事件参数</returns>
        UniTask<Dictionary<string, Dictionary<string, string>>> Execute(ILogEventInterceptConfiguration configuration, string eventName, Dictionary<string, string> parameters);
    }
    */
}
